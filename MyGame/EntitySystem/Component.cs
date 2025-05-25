using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.Data;
using MyGame.Level;
using MyGame.Utils;
using SpaceCup.Utils;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MyGame.EntitySystem;
internal abstract class Component()
{
    public Entity Owner { get; set; }
    public virtual void Update(float dt)
    {
        if (Owner == null || !Owner.IsAlive)
            return;
    }
    public virtual void Draw(SpriteBatch spriteBatch, float dt)
    {
        if (Owner == null || !Owner.IsAlive)
            return;
    }
}

internal class SpriteRenderer(int id) : Component
{
    public int Id = id;
    public SpriteEffects SpriteEffects { get; private set; }
    public bool IsFacingRight() => SpriteEffects == SpriteEffects.None;
    public override void Draw(SpriteBatch spriteBatch, float dt)
    {
        base.Draw(spriteBatch, dt);
        if (Owner.Vel.X > 0)
            SpriteEffects = SpriteEffects.None;
        if (Owner.Vel.X < 0)
            SpriteEffects = SpriteEffects.FlipHorizontally;

        spriteBatch.Draw(AssetManager.SpriteSheet,
            new Rectangle(Owner.Pos.ToPoint(), new Point(GameData.TILE_SIZE)),
            AssetManager.GetSrc(Id),
            Color.White,
            0, Vector2.Zero,
            SpriteEffects, 0);
        if (GameData.IsDebug)
        {
            DebugRect debugRect = new DebugRect(new Rectangle(Owner.Pos.ToPoint(), new(GameData.TILE_SIZE)), Color.Red);
            debugRect.DrawHollow(spriteBatch);
        }
    }
}


internal class Collider(Map map) : Component
{
    public Rectangle Rect;
    public Vector2 Offset;
    public Point? Dimensions = null;
    public Map Map { get; set; } = map;
    public bool IsGround { get; private set; }
    public event Action OnGrounded;
    public event Action OnCollision;
    private bool collided = false;
    public void Move(Vector2 amount)
    {
        collided = false;
        MoveX(amount.X);
        MoveY(amount.Y);
    }
    public void MoveX(float amount)
    {
        Rect = new Rectangle((Owner.Pos + Offset).ToPoint(), new(GameData.TILE_SIZE));
        if (Dimensions.HasValue)
            Rect.Size = Dimensions.Value;

        Rect.X += (int)(Math.Round(amount));
        List<Rectangle> rects = Map.GetNearRects(Rect.Location.ToVector2());
        foreach (Rectangle rect in rects)
        {
            if (Rect.Intersects(rect))
            {
                if (amount < 0)
                {
                    Rect.X = rect.Right;
                }
                if (amount > 0)
                {
                    Rect.X = rect.Left - Rect.Width;
                }
                collided = true;
            }
        }
        Owner.Pos = Rect.Location.ToVector2() - Offset;


    }
    public void MoveY(float amount)
    {
        Rect = new Rectangle((Owner.Pos + Offset).ToPoint(), new(GameData.TILE_SIZE));
        if (Dimensions.HasValue)
            Rect.Size = Dimensions.Value;
        Rect.Y += (int)MathF.Ceiling(amount);
        List<Rectangle> rects = Map.GetNearRects(Rect.Location.ToVector2());

        IsGround = false;
        foreach (Rectangle rect in rects)
        {
            if (Rect.Intersects(rect))
            {
                if (amount < 0)
                {
                    Rect.Y = rect.Bottom;
                }
                //Is Colliding Tile below Owner?
                if (amount >= 0)
                {
                    Rect.Y = rect.Top - Rect.Height;
                    IsGround = true;
                    OnGrounded?.Invoke();
                }
                collided = true;
            }
        }
        Owner.Pos = Rect.Location.ToVector2() - Offset;
        if (IsGround)
            Owner.Vel.Y = 0;
        if (collided)
            OnCollision?.Invoke();
    }

    public override void Draw(SpriteBatch spriteBatch, float dt)
    {
        base.Draw(spriteBatch, dt);
        if (GameData.IsDebug)
        {
            DebugRect debugRect = new DebugRect(Rect, IsGround ? Color.Aqua : Color.Green);
            debugRect.DrawHollow(spriteBatch);

            List<Rectangle> rects = Map.GetNearRects(Owner.Pos);
            foreach (var rect in rects)
            {
                debugRect = new DebugRect(rect, Color.DarkGreen);
                debugRect.DrawHollow(spriteBatch);
            }
        }
    }

}
internal class Controller() : Component
{
    bool onGroundSet = false;
    public override void Update(float dt)
    {
        base.Update(dt);
        bool isGround = Owner.GetComponent<Collider>().IsGround;
        Movement movement = Owner.GetComponent<Movement>();
        if (!onGroundSet)
        {
            Owner.GetComponent<Collider>().OnGrounded += () => movement.IsDoubleJump = true;
            onGroundSet = true;
        }
        Owner.Vel.X = 0;
        if (InputManager.IsKeyPressed(Keys.Space) && isGround)
        {
            Owner.Vel.Y -= Owner.GetComponent<Movement>().JumpPower;
        }
        if (InputManager.IsKeyDown(Keys.A))
        {
            Owner.Vel.X += -movement.Speed * (isGround ? 1f : 0.8f);
        }
        if (InputManager.IsKeyDown(Keys.D))
        {
            Owner.Vel.X += movement.Speed * (isGround ? 1f : 0.8f);

        }

        if (!InputManager.IsKeyPressed(Keys.E))
        {
            if (isGround)
            {
                if (Owner.Vel.X != 0)
                {
                    Owner.GetComponent<Animator>().SetCurrentAnimation("Run");
                }
                else
                {
                    Owner.GetComponent<Animator>().SetCurrentAnimation("Idle");
                }
            }
            else
            {
                Owner.GetComponent<Animator>().SetCurrentAnimation("Jump");
            }
        }
        else
        {
            if (Owner.HasComponent<Gun>())
            {
                Owner.GetComponent<Animator>().SetCurrentAnimation("Shoot");
                Owner.GetComponent<Gun>().Shoot();
            }
        }
        if (!isGround && movement.IsDoubleJump && InputManager.IsKeyPressed(Keys.Space))
        {
            movement.IsDoubleJump = false;
            if (Owner.Vel.Y <= 0)
                Owner.Vel.Y -= movement.JumpPower * movement.DoubleJumpFactor;
            else
                Owner.Vel.Y = movement.JumpPower * -movement.DoubleJumpFactor;

        }

    }
}
internal class Movement(float speed, float jumpPower, float gravity) : Component
{
    public float Speed { get; set; } = speed;
    public float JumpPower { get; set; } = jumpPower;
    public float TerminalVelocity = 5;

    public bool IsDoubleJump = false;

    public float DoubleJumpFactor = .8f;

    public override void Update(float dt)
    {
        base.Update(dt);
        Owner.Vel.X = Math.Clamp(Owner.Vel.X, -Speed, Speed);
        if (Owner.Vel.Y <= 0)
        {
            Owner.Vel.Y += gravity;
        }
        else
        {
            Owner.Vel.Y += gravity * 1.2f;
        }
        Owner.Vel.Y = Math.Clamp(Owner.Vel.Y, -MathF.Ceiling(JumpPower * (DoubleJumpFactor * .2f + 1)), TerminalVelocity);
        if (Owner.HasComponent<Collider>())
        {
            Owner.GetComponent<Collider>().Move(Owner.Vel);

        }
        else
        {
            Owner.Pos += Owner.Vel;
        }
    }


}

internal class Animator(float frameTime) : Component
{
    private float _timer { get; set; } = frameTime;
    public readonly float FrameTime = frameTime;
    Dictionary<string, Animation> Animations = new(4);
    public string CurrentAnimation = "";
    public string NextAnimation = "";
    public int CurrentFrame() => Animations[CurrentAnimation].CurrentFrame;
    public Animator AddAnimation(string name, bool waitTillEnd = false, params int[] frames)
    {
        Animations[name] = new Animation { FrameIds = frames, WaitTillEnd = waitTillEnd };
        if (Animations.Count == 1)
            CurrentAnimation = name;
        return this;
    }
    public Animator SetCurrentAnimation(string animation)
    {

        if (Animations[CurrentAnimation].WaitTillEnd)
        {
            NextAnimation = animation;
        }
        else
        {
            CurrentAnimation = animation;
        }
        return this;
    }
    public override void Update(float dt)
    {
        base.Update(dt);
        if (string.IsNullOrEmpty(CurrentAnimation))
            return;
        _timer -= dt;
        if (Animations[CurrentAnimation].WaitTillEnd)
        {
            Animations[CurrentAnimation].OnEnd += () => CurrentAnimation = NextAnimation;
        }
        if (_timer <= 0)
        {
            _timer = FrameTime;
            Animations[CurrentAnimation].ChangeFrame();

            Owner.GetComponent<SpriteRenderer>().Id = Animations[CurrentAnimation].CurrentFrame;

        }
    }
    internal class Animation()
    {
        public int[] FrameIds { get; set; }
        public int CurrentFrameIndex { get; set; }
        public bool WaitTillEnd = false;
        public event Action OnEnd;
        public void ChangeFrame()
        {
            CurrentFrameIndex++;
            CurrentFrameIndex %= FrameIds.Length;
            if (CurrentFrameIndex == 0)
                OnEnd?.Invoke();

        }
        public int CurrentFrame => FrameIds[CurrentFrameIndex];
    }

}
internal class EnemyAi() : Component
{
    public Rectangle CheckBox { get; set; }
    public override void Update(float dt)
    {
        base.Update(dt);
        var rects = Owner.GetComponent<Collider>().Map.GetNearRects(Owner.Pos);
        Vector2 pos = Owner.Pos + new Vector2(Math.Sign(Owner.Vel.X) * GameData.TILE_SIZE, GameData.TILE_SIZE);
        CheckBox = new Rectangle(pos.ToPoint(),
            new(GameData.TILE_SIZE));
        bool changeDirection = true;
        foreach (var item in rects)
        {
            if (item.Intersects(CheckBox))
                changeDirection = false;
        }
        if (changeDirection)
        {
            Owner.Vel.X *= -1;
        }

    }
    public override void Draw(SpriteBatch spriteBatch, float dt)
    {
        base.Draw(spriteBatch, dt);
        if (GameData.IsDebug)
        {
            DebugRect debugRect = new DebugRect(CheckBox, Color.Blue);
            debugRect.DrawHollow(spriteBatch);
        }
    }
}
internal class Bullet(bool isFacingRight) : Component
{
    private const float LIFE_TIME = 10;
    private float timer = 0;
    public override void Update(float dt)
    {
        base.Update(dt);
        timer += dt;
        if (LIFE_TIME <= timer)
        {
            Owner.Disable();
            return;
        }
        Owner.Vel.X = Owner.GetComponent<Movement>().Speed * (isFacingRight ? 1 : -1);
        Owner.GetComponent<Collider>().Offset = new Vector2(1, 4);
        Owner.GetComponent<Collider>().Dimensions = new Point(14, 8);
    }
}
internal class Gun(Action<Entity> AddBullet, int ammo) : Component
{
    public int Ammo = ammo;
    Vector2 shootingPos;
    public bool CanShoot => Ammo > 0;
    private Vector2 GetShootingPos()
    {
        Vector2 shootingPos = Owner.Pos;
        if (Owner.GetComponent<SpriteRenderer>().IsFacingRight())
            shootingPos.X += GameData.TILE_SIZE - 1;
        else
            shootingPos.X -= GameData.TILE_SIZE - 1;
        return shootingPos;
    }
    public void Shoot()
    {
        if (Ammo == 0)
            return;
        var newBullet = new Entity(GetShootingPos()).
            AddComponent(new SpriteRenderer(44)).
            AddComponent(new Movement(5, 0, 0)).
            AddComponent(new Collider(Owner.GetComponent<Collider>().Map))
            .AddComponent(new Bullet(Owner.GetComponent<SpriteRenderer>().IsFacingRight()));

        newBullet.GetComponent<Collider>().OnCollision += newBullet.Disable;

        AddBullet?.Invoke(newBullet);
        Ammo--;
    }
    public override void Draw(SpriteBatch spriteBatch, float dt)
    {
        base.Draw(spriteBatch, dt);
        if (!CanShoot)
            return;
        if (Owner.GetComponent<Animator>().CurrentAnimation != "Shoot" || Owner.GetComponent<SpriteRenderer>().Id == 42)
            return;
        shootingPos.X = (int)Owner.Pos.X +
            (Owner.GetComponent<SpriteRenderer>().IsFacingRight() ? GameData.TILE_SIZE - 1 : -GameData.TILE_SIZE + 1);

        shootingPos.Y = (int)(Owner.Pos.Y + 3f);
        spriteBatch.Draw(AssetManager.SpriteSheet, new Rectangle(shootingPos.ToPoint(), new Point(GameData.TILE_SIZE)),
            AssetManager.GetSrc(43), Color.White, 0,
            Vector2.Zero,
            Owner.GetComponent<SpriteRenderer>().SpriteEffects, 0);
    }


}
internal class TriggerArea(bool disableOnCollect = true) : Component
{
    public Rectangle Area;
    public event Action<Entity> OnTrigger;
    public override void Update(float dt)
    {
        base.Update(dt);
        Area = new Rectangle(Owner.Pos.ToPoint(), new Point(GameData.TILE_SIZE));
    }
    public void Trigger(Entity collector)
    {
        if (!collector.HasComponent<Collider>() || !collector.GetComponent<Collider>().Rect.Intersects(Area))
            return;

        OnTrigger?.Invoke(collector);
        if (disableOnCollect)
            Owner.Disable();
    }
}
internal class MovementParticles(Color particleColor) : Component
{
    public List<Particle> Particles = new(128);
    public float LifeTime = .3f;
    public override void Update(float dt)
    {
        base.Update(dt);
        if (Owner.GetComponent<Animator>().CurrentAnimation == "Run")
            Particles.Add(new Particle((int)Owner.Pos.X, (int)Owner.Pos.Y + GameData.TILE_SIZE)
            { Dir = new Vector2(-Math.Sign(Owner.Vel.X), -(float)(new Random().NextDouble())) });
        for (int i = 0; i < Particles.Count; i++)
        {
            Particles[i] = Particles[i].Update(dt);
        }
        Particles.RemoveAll(p => p.LifeTime > LifeTime);
    }
    public override void Draw(SpriteBatch spriteBatch, float dt)
    {
        base.Draw(spriteBatch, dt);
        foreach (var particle in Particles)
        {
            spriteBatch.Draw(AssetManager.Pixel, new Vector2(particle.X, particle.Y), null, particleColor * ((LifeTime - particle.LifeTime) / LifeTime),
                0, Vector2.Zero, 1 + particle.LifeTime * 5, SpriteEffects.None, 0);
        }
    }

}
public struct Particle(float x, float y, float speed = 1)
{
    public float X { get; set; } = x;
    public float Y { get; set; } = y;
    public float LifeTime { get; set; }
    public Vector2 Dir { get; set; }
    public float Speed = speed;
    public Particle Update(float dt)
    {
        LifeTime += dt;
        if (Dir.LengthSquared() > 0)
        {
            Dir = Vector2.Normalize(Dir);
        }
        X += (Dir.X) * Speed;
        Y += (Dir.Y) * Speed;
        return this;
    }

}
internal class ExplosionParticles : Component
{
    public Particle[] Particles;
    private Color _color;
    private float _speed;
    public ExplosionParticles(Color color, float speed, int particleCount)
    {
        Particles = new Particle[particleCount];
        _color = color;
        _speed = speed;
    }
    public float lifeTime = .75f;
    private bool init = false;
    public override void Update(float dt)
    {
        base.Update(dt);

        if (!init)
        {

            Random random = new Random();
            for (int i = 0; i < Particles.Length; i++)
            {
                Particles[i] = new Particle(Owner.Pos.X, Owner.Pos.Y, _speed * (float)random.NextDouble() + .1f);
                Particles[i].Dir = new Vector2((float)(random.NextDouble() * 2 - 1), (float)(random.NextDouble() * 2 - 1));

            }
            init = true;
        }
        for (int i = 0; i < Particles.Length; i++)
        {
            Particles[i] = Particles[i].Update(dt);
        }
        if (!Particles.Where(p => p.LifeTime < lifeTime).Any())
        {
            IsFinisehd = true;
        }
    }
    public bool IsFinisehd = false;
    public override void Draw(SpriteBatch spriteBatch, float dt)
    {
        base.Draw(spriteBatch, dt);

        foreach (var particle in Particles)
        {

            spriteBatch.Draw(AssetManager.Pixel, new Vector2(particle.X, particle.Y), null, _color * ((lifeTime - particle.LifeTime) / lifeTime),
                0, Vector2.Zero, 1 + particle.LifeTime * 5 * particle.Speed / _speed, SpriteEffects.None, 0);
        }

    }
}