﻿#region File Description

//-----------------------------------------------------------------------------
// Animation.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion

using Microsoft.Xna.Framework.Graphics;

namespace Platformer2D.Game
{
    /// <summary>
    /// Represents an animated texture.
    /// </summary>
    /// <remarks>
    /// Currently, this class assumes that each frame of animation is
    /// as wide as each animation is tall. The number of frames in the
    /// animation are inferred from this.
    /// </remarks>
    internal class Animation
    {
        /// <summary>
        /// Constructors a new animation.
        /// </summary>
        public Animation(Texture2D texture, float frameTime, bool isLooping)
        {
            Texture = texture;
            FrameTime = frameTime;
            IsLooping = isLooping;
        }

        /// <summary>
        /// All frames in the animation arranged horizontally.
        /// </summary>
        public Texture2D Texture { get; }

        /// <summary>
        /// Duration of time to show each frame.
        /// </summary>
        public float FrameTime { get; }

        /// <summary>
        /// When the end of the animation is reached, should it
        /// continue playing from the beginning?
        /// </summary>
        public bool IsLooping { get; }

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        public int FrameCount
        {
            get { return Texture.Width / FrameWidth; }
        }

        /// <summary>
        /// Gets the width of a frame in the animation.
        /// </summary>
        public int FrameWidth
        {
            // Assume square frames.
            get { return Texture.Height; }
        }

        /// <summary>
        /// Gets the height of a frame in the animation.
        /// </summary>
        public int FrameHeight
        {
            get { return Texture.Height; }
        }
    }
}