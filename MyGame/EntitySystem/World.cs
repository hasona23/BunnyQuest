using Microsoft.Xna.Framework.Graphics;
using MyGame.Data;
using MyGame.Level;
using MyGame.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyGame.EntitySystem;
internal class World
{
    public string Name;
    public bool Restart = false;
    public bool Finish;
    public Cam2D Cam;
    public Map Map;
    public Entity Player;
    public List<Entity> Enemies;
    public List<Entity> Bullets;
    public List<Entity> Items;
    public List<Entity> Particles;
    public List<Entity> Clouds;
    private float _particleSpeed = 1;
    private int _particleCount = 48;
    private Random _rand = new Random();
    public void AddNewBullet(Entity bullet)
    {
        bullet.OnDisable += () => Particles.Add(new Entity(bullet.Pos).
            AddComponent(new ExplosionParticles(Color.MonoGameOrange, _particleSpeed, _particleCount)));
        Bullets.Add(bullet);
        AssetManager.Sounds["Shoot"].Pitch = _rand.NextSingle() - .5f;

        AssetManager.Sounds["Shoot"].Play();
    }
    public World(string map)
    {
        Map = new Map(map);
        Cam = new Cam2D(Vector2.Zero, Vector2.Zero, new Vector2(Map.Width, Map.Height) * GameData.TILE_SIZE);
        Name = Path.GetFileNameWithoutExtension(map);

        InitEntities();
    }
    public void Reset()
    {
        InitEntities();
        Restart = false;
    }

    private void InitEntities()
    {
        Enemies = new List<Entity>();
        Bullets = new List<Entity>();
        Items = new List<Entity>();
        Particles = new List<Entity>(1 + Map.Enemies().Length * 2 + 1 + Map.Items().Length);
        Obj playerObj = Map.Player();
        Player = new Entity(new Vector2(playerObj.X, playerObj.Y));


        int cloudsCount = 16;
        Clouds = new List<Entity>(cloudsCount);

        for (int i = 0; i < cloudsCount; i++)
        {
            Clouds.Add(new Entity(new Vector2(Map.Width * _rand.NextSingle() * GameData.TILE_SIZE, _rand.Next(Map.Height) * GameData.TILE_SIZE))
                .AddComponent(new SpriteRenderer(11))
                .AddComponent(new Movement(MathF.Max(1 - _rand.NextSingle(), .25f), 0, 0)));
            Clouds[i].Vel.X = -(Clouds[i].GetComponent<Movement>().Speed);
        }

        Player.AddComponent(new SpriteRenderer(playerObj.Id))
                .AddComponent(new Movement(2, 5, .25f))
                .AddComponent(new Controller())
                .AddComponent(new Animator(.2f)
                    .AddAnimation("Idle", false, 45)
                    .AddAnimation("Run", false, 45, 46)
                    .AddAnimation("Jump", false, 46))
                .AddComponent(new Collider(Map))
                .AddComponent(new MovementParticles(Color.DarkOrange)); ;
        if (playerObj.Id == 40)
        {
            Player.GetComponent<Animator>().AddAnimation("Idle", false, 40)
                                .AddAnimation("Run", false, 40, 41)
                                .AddAnimation("Jump", false, 41).AddAnimation("Shoot", true, 40, 42);
            Player.AddComponent(new Gun(AddNewBullet, Map.Enemies().Length + 1));
        }
        Player.OnDisable += () =>
                Particles.Add(new Entity(Player.Pos).AddComponent(new ExplosionParticles(Color.SkyBlue, _particleSpeed, _particleCount)));
        ;
        foreach (var obj in Map.Enemies())
        {
            Entity enemy = new Entity(new Vector2(obj.X, obj.Y))
                .AddComponent(new SpriteRenderer(obj.Id))
                .AddComponent(new Animator(.2f).AddAnimation("Move", false, obj.Id, obj.Id + 1))
                .AddComponent(new Collider(Map))
                .AddComponent(new Movement(1, 0, .25f))
                .AddComponent(new EnemyAi());
            enemy.Vel.X = 2;
            Enemies.Add(enemy);
            Color particleColor = Color.Red;
            if (obj.Id == 51)
                particleColor = Color.Yellow;
            if (obj.Id == 53)
                particleColor = Color.DarkCyan;
            enemy.OnDisable += () =>
            {
                Particles.Add(new Entity(enemy.Pos).AddComponent(new ExplosionParticles(particleColor, _particleSpeed, _particleCount)));
                AssetManager.Sounds["Explode"].Pitch = _rand.NextSingle() - .5f;
                AssetManager.Sounds["Explode"].Play();
            };

        }
        foreach (var obj in Map.Items())
        {
            Entity item = new Entity(new Vector2(obj.X, obj.Y))
                .AddComponent(new SpriteRenderer(obj.Id))
                .AddComponent(new TriggerArea());
            if (obj.Id == 50)
                item.GetComponent<TriggerArea>().OnTrigger += (entity) =>
                    {
                        entity.GetComponent<Animator>().AddAnimation("Idle", false, 40)
                            .AddAnimation("Run", false, 40, 41)
                            .AddAnimation("Jump", false, 41).AddAnimation("Shoot", true, 40, 42);
                        entity.AddComponent(new Gun(AddNewBullet, Map.Enemies().Length + 1));
                        Particles.Add(new Entity(item.Pos).AddComponent(new ExplosionParticles(Color.OrangeRed, _particleSpeed, _particleCount)));
                        AssetManager.Sounds["PowerUp"].Pitch = _rand.NextSingle() - .5f;
                        AssetManager.Sounds["PowerUp"].Play();
                    }
            ;
            Items.Add(item);
        }
        foreach (var obj in Map.Checkpoints())
        {
            Entity item = new Entity(new Vector2(obj.X, obj.Y))
                   .AddComponent(new SpriteRenderer(obj.Id))
                   .AddComponent(new TriggerArea());
            if (obj.Id == 36 || obj.Id == 37)
            {
                item.GetComponent<TriggerArea>().OnTrigger += (entity) =>
                {
                    Finish = true;
                }
            ;
            }
            Items.Add(item);
        }

    }


    public void Update(float dt)
    {

        Player.Update(dt);
        foreach (var item in Clouds)
        {

            item.Update(dt);
            if (item.Pos.X < -GameData.TILE_SIZE)
            {
                item.Pos = new Vector2((Map.Width + 1) * GameData.TILE_SIZE, _rand.Next(Map.Height) * GameData.TILE_SIZE);

                item.GetComponent<Movement>().Speed = MathF.Max(1 - _rand.NextSingle(), .25f);
                item.Vel.X = -(item.GetComponent<Movement>().Speed);
                ;

            }

        }
        foreach (var enemy in Enemies)
        {
            enemy.Update(dt);
            foreach (var bullet in Bullets)
            {
                if (bullet.HasComponent<Collider>() && bullet.GetComponent<Collider>().Rect.Intersects(enemy.GetComponent<Collider>().Rect))
                {
                    enemy.Disable();
                    bullet.Disable();
                }
            }
            if (enemy.IsAlive && Player.IsAlive && enemy.HasComponent<Collider>() &&
                enemy.GetComponent<Collider>().Rect.Intersects(Player.GetComponent<Collider>().Rect))
            {
                //TODO: DEATH PLAYER
                //Example Death screen,some particles
                //Check if jumped on enemy
                if ((enemy.GetComponent<Collider>().Rect.Top
                    - Player.GetComponent<Collider>().Rect.Bottom) <= 1 &&
                    (enemy.GetComponent<Collider>().Rect.Top - Player.GetComponent<Collider>().Rect.Bottom) >= -6)
                {
                    enemy.Disable();
                }
                else
                {
                    Player.Disable();

                    Main.GameState = GameState.Death;
                }
            }
            if (!enemy.IsAlive)
                enemy.Destroy();
        }
        foreach (var bullet in Bullets)
        {
            bullet.Update(dt);

            if (!bullet.IsAlive)
                bullet.Destroy();
        }
        foreach (var item in Items)
        {
            item.Update(dt);
            if (!item.IsAlive)
                item.Destroy();
            item.GetComponent<TriggerArea>().Trigger(Player);
        }
        foreach (var item in Particles)
        {
            item.Update(dt);
        }
        Particles.RemoveAll(e => !e.HasComponent<ExplosionParticles>() || e.GetComponent<ExplosionParticles>().IsFinisehd);


        Bullets.RemoveAll(b => !b.IsAlive);
        Enemies.RemoveAll(e => !e.IsAlive);
        Items.RemoveAll(i => !i.IsAlive);
        //Player.Pos = Vector2.Clamp(Player.Pos, Vector2.Zero, new Vector2(Map.Width, Map.Height) * GameData.TILE_SIZE - new Vector2(GameData.TILE_SIZE));
        if (Player.IsAlive && Player.Pos.Y > Map.Height * GameData.TILE_SIZE - GameData.TILE_SIZE)
        {

            Player.Disable();

            Main.GameState = GameState.Death;
        }
        Cam.Follow(Player.Pos);

        if (Restart)
        {
            InitEntities();
            Restart = false;

        }
    }

    public void Draw(SpriteBatch spriteBatch, float dt)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Cam.GetCamMatrix());


        foreach (var item in Clouds)
        {
            item.Draw(spriteBatch, dt);
        }
        Map.Draw(spriteBatch);
        Player.Draw(spriteBatch, dt);
        foreach (var entity in Enemies)
        {
            entity.Draw(spriteBatch, dt);
        }
        foreach (var bullet in Bullets)
        {
            bullet.Draw(spriteBatch, dt);
        }
        foreach (var item in Items)
        {
            item.Draw(spriteBatch, dt);
        }
        foreach (var item in Particles)
        {
            item.Draw(spriteBatch, dt);
        }

        spriteBatch.End();

    }
}
