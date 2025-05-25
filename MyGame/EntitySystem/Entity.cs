using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyGame.EntitySystem;
internal class Entity(Vector2 pos)
{
    public Vector2 Pos = pos;
    public Vector2 Vel;
    public bool IsAlive = true;

    public List<Component> Components { get; set; } = new List<Component>();

    public T GetComponent<T>() where T : Component
    {
        return Components.OfType<T>().FirstOrDefault();
    }
    public Entity AddComponent(Component component)
    {
        Components.Add(component);
        component.Owner = this;
        return this;
    }
    public void RemoveComponent(Component component)
    {
        Components.Remove(component);
    }
    public void RemoveComponent<T>() where T : Component
    {
        Components.RemoveAll(c =>
        {
            if (c as T != null)
            {
                c.Owner = null;
                return true;
            }
            return false;
        });
    }

    public bool HasComponent<T>() where T : Component
    {
        return GetComponent<T>() != null;
    }
    public void Update(float dt)
    {
        if (!IsAlive)
            return;
        foreach (var component in Components)
        {
            component.Update(dt);
        }

    }


    public void Draw(SpriteBatch spriteBatch, float dt)
    {
        if (!IsAlive)
            return;
        foreach (var component in Components)
        {
            component.Draw(spriteBatch, dt);
        }
    }
    public void Disable()
    {
        IsAlive = false;
        OnDisable?.Invoke();
    }
    public event Action OnDisable;
    public void Destroy()
    {
        foreach (var component in Components)
        {
            component.Owner = null;
        }
        Components.Clear();

    }
}
