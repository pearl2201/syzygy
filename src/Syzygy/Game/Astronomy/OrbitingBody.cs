using System;
using System.Linq;
using amulware.Graphics;
using Bearded.Utilities.Math;
using Bearded.Utilities.SpaceTime;
using Syzygy.Rendering;
using TimeSpan = Bearded.Utilities.SpaceTime.TimeSpan;

namespace Syzygy.Game.Astronomy
{
    sealed class OrbitingBody : GameObject<IBody>, IBody
    {
        private readonly IBody parent;
        private readonly Radius orbitRadius;
        private Direction2 orbitDirection;
        private readonly Radius radius;
        private readonly float mass;
        private readonly Color color;
        private readonly float maxHealth;

        private float health;

        private readonly Angle angularVelocity;

        private Position2 center;

        public OrbitingBody(GameState game, Id<IBody> id, IBody parent,
            Radius orbitRadius, Direction2 orbitDirection, Radius radius,
            float mass, Color color, float health)
            : base(game, id)
        {
            this.parent = parent;
            this.orbitRadius = orbitRadius;
            this.orbitDirection = orbitDirection;
            this.radius = radius;
            this.mass = mass;
            this.color = color;
            this.health = health;
            this.maxHealth = health;

            this.center = this.calculatePosition();

            this.angularVelocity =
                ((Constants.G * parent.Mass / orbitRadius.NumericValue).Sqrted() / orbitRadius.NumericValue).Radians();
        }

        public OrbitingBody(GameState game, Id<IBody> id, Id<IBody> parentId,
            Radius orbitRadius, Direction2 orbitDirection, Radius radius, float mass, Color color, float health)
            : this(game, id, game.Bodies[parentId], orbitRadius, orbitDirection, radius, mass, color, health)
        {
        }

        private Position2 calculatePosition()
        {
            return new Position2(this.parent.Shape.Center.Vector + this.orbitDirection.Vector * this.orbitRadius.NumericValue);
        }

        public Circle Shape { get { return new Circle(this.center, this.radius); } }
        public float Mass { get { return this.mass; } }

        public Velocity2 Velocity
        {
            get
            {
                return new Velocity2(this.orbitDirection.Vector.PerpendicularLeft
                    * (this.angularVelocity.Radians * this.orbitRadius.NumericValue));
            }
        }

        public float HealthPercentage { get { return this.health / this.maxHealth; } }

        public void DealDamage(float damage)
        {
            if (damage <= 0)
                return;

            var eco = this.game.Economies.FirstOrDefault(e => e.Body == this);

            if (eco != null)
                damage *= eco.DamageFactor;

            this.health = Math.Max(this.health - damage, 0);
        }

        public override void Update(TimeSpan t)
        {
            this.orbitDirection = Direction2.Zero + this.angularVelocity * (float)this.game.Time.NumericValue;
            this.center = this.calculatePosition();
        }

        public override void Draw(GeometryManager geos)
        {
            geos.Primitives.Color = this.color;
            geos.Primitives.DrawCircle(this.center.Vector, this.radius.NumericValue);
        }

    }
}
