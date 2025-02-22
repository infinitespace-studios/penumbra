﻿#region File Description

//-----------------------------------------------------------------------------
// Gem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;

namespace Platformer2D.Game
{
    /// <summary>
    /// A valuable item the player can collect.
    /// </summary>
    internal class Gem
    {
        public const int PointValue = 30;
        private const float LightOscillationSpeed = 1.5f;
        private static readonly Random random = new Random();

        private static readonly Vector2 LightScale = new Vector2(80);
        public readonly Color Color = Color.Yellow;

        // The gem is animated from a base position along the Y axis.
        private readonly Vector2 basePosition;
        private float bounce;
        private SoundEffect collectedSound;
        private Vector2 origin;
        private float oscillationProgress = (float) random.NextDouble();

        private Texture2D texture;

        /// <summary>
        /// Constructs a new gem.
        /// </summary>
        public Gem(Level level, Vector2 position)
        {
            Level = level;
            basePosition = position;

            LoadContent();
        }

        public PointLight Light { get; } = new PointLight
        {
            Scale = LightScale,
            Color = Color.Yellow,
            CastsShadows = false
        };

        public Level Level { get; }

        /// <summary>
        /// Gets the current position of this gem in world space.
        /// </summary>
        public Vector2 Position
        {
            get { return basePosition + new Vector2(0.0f, bounce); }
        }

        /// <summary>
        /// Gets a circle which bounds this gem in world space.
        /// </summary>
        public Circle BoundingCircle
        {
            get { return new Circle(Position, Tile.Width / 3.0f); }
        }

        /// <summary>
        /// Loads the gem texture and collected sound.
        /// </summary>
        public void LoadContent()
        {
            texture = Level.Content.Load<Texture2D>("Sprites/Gem");
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            collectedSound = Level.Content.Load<SoundEffect>("Sounds/GemCollected");
        }

        /// <summary>
        /// Bounces up and down in the air to entice players to collect them.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Bounce control constants
            const float BounceHeight = 0.18f;
            const float BounceRate = 3.0f;
            const float BounceSync = -0.75f;

            // Bounce along a sine curve over time.
            // Include the X coordinate so that neighboring gems bounce in a nice wave pattern.            
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            bounce = (float) Math.Sin(t) * BounceHeight * texture.Height;

            Light.Position = Position;
            oscillationProgress += (float) gameTime.ElapsedGameTime.TotalSeconds / LightOscillationSpeed;
            if (oscillationProgress >= 1)
            {
                oscillationProgress -= 1;
            }
            Light.Scale = (float) Math.Sin(oscillationProgress * Math.PI) * LightScale * 0.3f + LightScale * 0.7f;
        }

        /// <summary>
        /// Called when this gem has been collected by a player and removed from the level.
        /// </summary>
        /// <param name="collectedBy">
        /// The player who collected this gem. Although currently not used, this parameter would be
        /// useful for creating special powerup gems. For example, a gem could make the player invincible.
        /// </param>
        public void OnCollected(Player collectedBy)
        {
            collectedSound.Play();
        }

        /// <summary>
        /// Draws a gem in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}