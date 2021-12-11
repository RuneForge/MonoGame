using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;

namespace MonoGame.Tests.Framework
{
    /// <summary>
    /// Tests for enum compatibility with XNA(here is only XNA enum members, extensions are not included).
    /// </summary>
    class EnumConformingTest
    {
        #region MonoGame.Framework

        [Test]
        public void ContainmentTypeEnum()
        {
            Assert.AreEqual(0, (int)ContainmentType.Disjoint);
            Assert.AreEqual(1, (int)ContainmentType.Contains);
            Assert.AreEqual(2, (int)ContainmentType.Intersects);
        }

        [Test]
        public void CurveContinuityEnum()
        {
            Assert.AreEqual(0, (int)CurveContinuity.Smooth);
            Assert.AreEqual(1, (int)CurveContinuity.Step);
        }

        [Test]
        public void CurveLoopTypeEnum()
        {
            Assert.AreEqual(0, (int)CurveLoopType.Constant);
            Assert.AreEqual(1, (int)CurveLoopType.Cycle);
            Assert.AreEqual(2, (int)CurveLoopType.CycleOffset);
            Assert.AreEqual(3, (int)CurveLoopType.Oscillate);
            Assert.AreEqual(4, (int)CurveLoopType.Linear);
        }

        [Test]
        public void CurveTangentEnum()
        {
            Assert.AreEqual(0, (int)CurveTangent.Flat);
            Assert.AreEqual(1, (int)CurveTangent.Linear);
            Assert.AreEqual(2, (int)CurveTangent.Smooth);
        }

        [Test]
        public void DisplayOrientationEnum()
        {
            Assert.AreEqual(0, (int)DisplayOrientations.Default);
            Assert.AreEqual(1, (int)DisplayOrientations.LandscapeLeft);
            Assert.AreEqual(2, (int)DisplayOrientations.LandscapeRight);
            Assert.AreEqual(4, (int)DisplayOrientations.Portrait);
        }

        [Test]
        public void PlaneIntersectionTypeEnum()
        {
            Assert.AreEqual(0, (int)PlaneIntersectionType.Front);
            Assert.AreEqual(1, (int)PlaneIntersectionType.Back);
            Assert.AreEqual(2, (int)PlaneIntersectionType.Intersecting);
        }

        [Test]
        public void PlayerIndexEnum()
        {
            Assert.AreEqual(0, (int)PlayerIndex.One);
            Assert.AreEqual(1, (int)PlayerIndex.Two);
            Assert.AreEqual(2, (int)PlayerIndex.Three);
            Assert.AreEqual(3, (int)PlayerIndex.Four);
        }

        #endregion

        #region MonoGame.Framework.Graphics

        [Test]
        public void BlendEnum()
        {
            Assert.AreEqual(0, (int)Blend.One);
            Assert.AreEqual(1, (int)Blend.Zero);
            Assert.AreEqual(2, (int)Blend.SourceColor);
            Assert.AreEqual(3, (int)Blend.InverseSourceColor);
            Assert.AreEqual(4, (int)Blend.SourceAlpha);
            Assert.AreEqual(5, (int)Blend.InverseSourceAlpha);
            Assert.AreEqual(6, (int)Blend.DestinationColor);
            Assert.AreEqual(7, (int)Blend.InverseDestinationColor);
            Assert.AreEqual(8, (int)Blend.DestinationAlpha);
            Assert.AreEqual(9, (int)Blend.InverseDestinationAlpha);
            Assert.AreEqual(10, (int)Blend.BlendFactor);
            Assert.AreEqual(11, (int)Blend.InverseBlendFactor);
            Assert.AreEqual(12, (int)Blend.SourceAlphaSaturation);
        }

        [Test]
        public void BlendFunctionEnum()
        {
            Assert.AreEqual(0, (int)BlendFunction.Add);
            Assert.AreEqual(1, (int)BlendFunction.Subtract);
            Assert.AreEqual(2, (int)BlendFunction.ReverseSubtract);
            Assert.AreEqual(3, (int)BlendFunction.Min);
            Assert.AreEqual(4, (int)BlendFunction.Max);
        }

        [Test]
        public void BufferUsageEnum()
        {
            Assert.AreEqual(0, (int)BufferUsage.None);
            Assert.AreEqual(1, (int)BufferUsage.WriteOnly);
        }

        [Test]
        public void ClearOptionsEnum()
        {
            Assert.AreEqual(1, (int)ClearOptions.Target);
            Assert.AreEqual(2, (int)ClearOptions.DepthBuffer);
            Assert.AreEqual(4, (int)ClearOptions.Stencil);
        }

        [Test]
        public void ColorWriteChannelsEnum()
        {
            Assert.AreEqual(0, (int)ColorWriteChannels.None);
            Assert.AreEqual(1, (int)ColorWriteChannels.Red);
            Assert.AreEqual(2, (int)ColorWriteChannels.Green);
            Assert.AreEqual(4, (int)ColorWriteChannels.Blue);
            Assert.AreEqual(8, (int)ColorWriteChannels.Alpha);
            Assert.AreEqual(15, (int)ColorWriteChannels.All);
        }

        [Test]
        public void CompareFunctionEnum()
        {
            Assert.AreEqual(0, (int)CompareFunction.Always);
            Assert.AreEqual(1, (int)CompareFunction.Never);
            Assert.AreEqual(2, (int)CompareFunction.Less);
            Assert.AreEqual(3, (int)CompareFunction.LessEqual);
            Assert.AreEqual(4, (int)CompareFunction.Equal);
            Assert.AreEqual(5, (int)CompareFunction.GreaterEqual);
            Assert.AreEqual(6, (int)CompareFunction.Greater);
            Assert.AreEqual(7, (int)CompareFunction.NotEqual);
        }

        [Test]
        public void CubeMapFaceEnum()
        {
            Assert.AreEqual(0, (int)CubeMapFace.PositiveX);
            Assert.AreEqual(1, (int)CubeMapFace.NegativeX);
            Assert.AreEqual(2, (int)CubeMapFace.PositiveY);
            Assert.AreEqual(3, (int)CubeMapFace.NegativeY);
            Assert.AreEqual(4, (int)CubeMapFace.PositiveZ);
            Assert.AreEqual(5, (int)CubeMapFace.NegativeZ);
        }

        [Test]
        public void CullModeEnum()
        {
            Assert.AreEqual(0, (int)CullMode.None);
            Assert.AreEqual(1, (int)CullMode.CullClockwiseFace);
            Assert.AreEqual(2, (int)CullMode.CullCounterClockwiseFace);
        }

        [Test]
        public void DepthFormatEnum()
        {
            Assert.AreEqual(0, (int)DepthFormat.None);
            Assert.AreEqual(1, (int)DepthFormat.Depth16);
            Assert.AreEqual(2, (int)DepthFormat.Depth24);
            Assert.AreEqual(3, (int)DepthFormat.Depth24Stencil8);
        }

        [Test]
        public void EffectParameterClassEnum()
        {
            Assert.AreEqual(0, (int)EffectParameterClass.Scalar);
            Assert.AreEqual(1, (int)EffectParameterClass.Vector);
            Assert.AreEqual(2, (int)EffectParameterClass.Matrix);
            Assert.AreEqual(3, (int)EffectParameterClass.Object);
            Assert.AreEqual(4, (int)EffectParameterClass.Struct);
        }

        [Test]
        public void EffectParameterTypeEnum()
        {
            Assert.AreEqual(0, (int) EffectParameterType.Void);
            Assert.AreEqual(1, (int)EffectParameterType.Bool);
            Assert.AreEqual(2, (int)EffectParameterType.Int32);
            Assert.AreEqual(3, (int)EffectParameterType.Single);
            Assert.AreEqual(4, (int)EffectParameterType.String);
            Assert.AreEqual(5, (int)EffectParameterType.Texture);
            Assert.AreEqual(6, (int)EffectParameterType.Texture1D);
            Assert.AreEqual(7, (int)EffectParameterType.Texture2D);
            Assert.AreEqual(8, (int)EffectParameterType.Texture3D);
            Assert.AreEqual(9, (int)EffectParameterType.TextureCube);
        }

        [Test]
        public void FillModeEnum()
        {
            Assert.AreEqual(0, (int)FillMode.Solid);
            Assert.AreEqual(1, (int)FillMode.WireFrame);         
        }

        [Test]
        public void GraphicsDeviceStatusEnum()
        {
            Assert.AreEqual(0, (int)GraphicsDeviceStatus.Normal);
            Assert.AreEqual(1, (int)GraphicsDeviceStatus.Lost);
            Assert.AreEqual(2, (int)GraphicsDeviceStatus.NotReset);
        }

        [Test]
        public void GraphicsProfileEnum()
        {
            Assert.AreEqual(0, (int)GraphicsProfile.Reach);
            Assert.AreEqual(1, (int)GraphicsProfile.HiDef);
        }

        [Test]
        public void IndexElementSizeEnum()
        {
            Assert.AreEqual(0, (int)IndexElementSize.SixteenBits);
            Assert.AreEqual(1, (int)IndexElementSize.ThirtyTwoBits);
        }

        [Test]
        public void PresentIntervalEnum()
        {
            Assert.AreEqual(0, (int)PresentInterval.Default);
            Assert.AreEqual(1, (int)PresentInterval.One);
            Assert.AreEqual(2, (int)PresentInterval.Two);
            Assert.AreEqual(3, (int)PresentInterval.Immediate);
        }

        [Test]
        public void PrimitiveTypeEnum()
        {
            Assert.AreEqual(0, (int)PrimitiveType.TriangleList);
            Assert.AreEqual(1, (int)PrimitiveType.TriangleStrip);
            Assert.AreEqual(2, (int)PrimitiveType.LineList);
            Assert.AreEqual(3, (int)PrimitiveType.LineStrip);
        }

        [Test]
        public void RenderTargetUsageEnum()
        {
            Assert.AreEqual(0, (int)RenderTargetUsage.DiscardContents);
            Assert.AreEqual(1, (int)RenderTargetUsage.PreserveContents);
            Assert.AreEqual(2, (int)RenderTargetUsage.PlatformContents);
        }

        [Test]
        public void SetDataOptionsEnum()
        {
            Assert.AreEqual(0, (int)SetDataOptions.None);
            Assert.AreEqual(1, (int)SetDataOptions.Discard);
            Assert.AreEqual(2, (int)SetDataOptions.NoOverwrite);
        }

        [Test]
        public void SpriteSortModeEnum()
        {
            Assert.AreEqual(0, (int)SpriteSortMode.Deferred);
            Assert.AreEqual(1, (int)SpriteSortMode.Immediate);
            Assert.AreEqual(2, (int)SpriteSortMode.Texture);
            Assert.AreEqual(3, (int)SpriteSortMode.BackToFront);
            Assert.AreEqual(4, (int)SpriteSortMode.FrontToBack);
        }

        [Test]
        public void StencilOperationEnum()
        {
            Assert.AreEqual(0, (int)StencilOperation.Keep);
            Assert.AreEqual(1, (int)StencilOperation.Zero);
            Assert.AreEqual(2, (int)StencilOperation.Replace);
            Assert.AreEqual(3, (int)StencilOperation.Increment);
            Assert.AreEqual(4, (int)StencilOperation.Decrement);
            Assert.AreEqual(5, (int)StencilOperation.IncrementSaturation);
            Assert.AreEqual(6, (int)StencilOperation.DecrementSaturation);
            Assert.AreEqual(7, (int)StencilOperation.Invert);
        }

        [Test]
        public void SurfaceFormateEnum()
        {
            Assert.AreEqual(0, (int)SurfaceFormat.Color);
            Assert.AreEqual(1, (int)SurfaceFormat.Bgr565);
            Assert.AreEqual(2, (int)SurfaceFormat.Bgra5551);
            Assert.AreEqual(3, (int)SurfaceFormat.Bgra4444);
            Assert.AreEqual(4, (int)SurfaceFormat.Dxt1);
            Assert.AreEqual(5, (int)SurfaceFormat.Dxt3);
            Assert.AreEqual(6, (int)SurfaceFormat.Dxt5);
            Assert.AreEqual(7, (int)SurfaceFormat.NormalizedByte2);
            Assert.AreEqual(8, (int)SurfaceFormat.NormalizedByte4);
            Assert.AreEqual(9, (int)SurfaceFormat.Rgba1010102);
            Assert.AreEqual(10, (int)SurfaceFormat.Rg32);
            Assert.AreEqual(11, (int)SurfaceFormat.Rgba64);
            Assert.AreEqual(12, (int)SurfaceFormat.Alpha8);
            Assert.AreEqual(13, (int)SurfaceFormat.Single);
            Assert.AreEqual(14, (int)SurfaceFormat.Vector2);
            Assert.AreEqual(15, (int)SurfaceFormat.Vector4);
            Assert.AreEqual(16, (int)SurfaceFormat.HalfSingle);
            Assert.AreEqual(17, (int)SurfaceFormat.HalfVector2);
            Assert.AreEqual(18, (int)SurfaceFormat.HalfVector4);
            Assert.AreEqual(19, (int)SurfaceFormat.HdrBlendable);
        }

        [Test]
        public void TextureAddressModeEnum()
        {
            Assert.AreEqual(0, (int)TextureAddressMode.Wrap);
            Assert.AreEqual(1, (int)TextureAddressMode.Clamp);
            Assert.AreEqual(2, (int)TextureAddressMode.Mirror);
        }

        [Test]
        public void TextureFilterEnum()
        {
            Assert.AreEqual(0, (int)TextureFilter.Linear);
            Assert.AreEqual(1, (int)TextureFilter.Point);
            Assert.AreEqual(2, (int)TextureFilter.Anisotropic);
            Assert.AreEqual(3, (int)TextureFilter.LinearMipPoint);
            Assert.AreEqual(4, (int)TextureFilter.PointMipLinear);
            Assert.AreEqual(5, (int)TextureFilter.MinLinearMagPointMipLinear);
            Assert.AreEqual(6, (int)TextureFilter.MinLinearMagPointMipPoint);
            Assert.AreEqual(7, (int)TextureFilter.MinPointMagLinearMipLinear);
            Assert.AreEqual(8, (int)TextureFilter.MinPointMagLinearMipPoint);
        }

        [Test]
        public void VertexElementFormatEnum()
        {
            Assert.AreEqual(0, (int)VertexElementFormat.Single);
            Assert.AreEqual(1, (int)VertexElementFormat.Vector2);
            Assert.AreEqual(2, (int)VertexElementFormat.Vector3);
            Assert.AreEqual(3, (int)VertexElementFormat.Vector4);
            Assert.AreEqual(4, (int)VertexElementFormat.Color);
            Assert.AreEqual(5, (int)VertexElementFormat.Byte4);
            Assert.AreEqual(6, (int)VertexElementFormat.Short2);
            Assert.AreEqual(7, (int)VertexElementFormat.Short4);
            Assert.AreEqual(8, (int)VertexElementFormat.NormalizedShort2);
            Assert.AreEqual(9, (int)VertexElementFormat.NormalizedShort4);
            Assert.AreEqual(10, (int)VertexElementFormat.HalfVector2);
            Assert.AreEqual(11, (int)VertexElementFormat.HalfVector4);
        }

        [Test]
        public void VertexElementUsageEnum()
        {
            Assert.AreEqual(0, (int)VertexElementUsage.Position);
            Assert.AreEqual(1, (int)VertexElementUsage.Color);
            Assert.AreEqual(2, (int)VertexElementUsage.TextureCoordinate);
            Assert.AreEqual(3, (int)VertexElementUsage.Normal);
            Assert.AreEqual(4, (int)VertexElementUsage.Binormal);
            Assert.AreEqual(5, (int)VertexElementUsage.Tangent);
            Assert.AreEqual(6, (int)VertexElementUsage.BlendIndices);
            Assert.AreEqual(7, (int)VertexElementUsage.BlendWeight);
            Assert.AreEqual(8, (int)VertexElementUsage.Depth);
            Assert.AreEqual(9, (int)VertexElementUsage.Fog);
            Assert.AreEqual(10, (int)VertexElementUsage.PointSize);
            Assert.AreEqual(11, (int)VertexElementUsage.Sample);
            Assert.AreEqual(12, (int)VertexElementUsage.TessellateFactor);
        }

        #endregion

        #region MonoGame.Framework.Input

        [Test]
        public void ButtonsEnum()
        {
            Assert.AreEqual(1, (int)GamePadButtons.DPadUp);
            Assert.AreEqual(2, (int)GamePadButtons.DPadDown);
            Assert.AreEqual(4, (int)GamePadButtons.DPadLeft);
            Assert.AreEqual(8, (int)GamePadButtons.DPadRight);
            Assert.AreEqual(16, (int)GamePadButtons.Start);
            Assert.AreEqual(32, (int)GamePadButtons.Back);
            Assert.AreEqual(64, (int)GamePadButtons.LeftStick);
            Assert.AreEqual(128, (int)GamePadButtons.RightStick);
            Assert.AreEqual(256, (int)GamePadButtons.LeftShoulder);
            Assert.AreEqual(512, (int)GamePadButtons.RightShoulder);
            Assert.AreEqual(2048, (int)GamePadButtons.BigButton);
            Assert.AreEqual(4096, (int)GamePadButtons.A);
            Assert.AreEqual(8192, (int)GamePadButtons.B);
            Assert.AreEqual(16384, (int)GamePadButtons.X);
            Assert.AreEqual(32768, (int)GamePadButtons.Y);
            Assert.AreEqual(2097152, (int)GamePadButtons.LeftThumbstickLeft);
            Assert.AreEqual(4194304, (int)GamePadButtons.RightTrigger);
            Assert.AreEqual(8388608, (int)GamePadButtons.LeftTrigger);
            Assert.AreEqual(16777216, (int)GamePadButtons.RightThumbstickUp);
            Assert.AreEqual(33554432, (int)GamePadButtons.RightThumbstickDown);
            Assert.AreEqual(67108864, (int)GamePadButtons.RightThumbstickRight);
            Assert.AreEqual(134217728, (int)GamePadButtons.RightThumbstickLeft);
            Assert.AreEqual(268435456, (int)GamePadButtons.LeftThumbstickUp);
            Assert.AreEqual(536870912, (int)GamePadButtons.LeftThumbstickDown);
            Assert.AreEqual(1073741824, (int)GamePadButtons.LeftThumbstickRight);
        }

        [Test]
        public void ButtonStateEnum()
        {
            Assert.AreEqual(0, (int)ButtonState.Released);
            Assert.AreEqual(1, (int)ButtonState.Pressed);
        }

        [Test]
        public void GamePadTypeEnum()
        {
            Assert.AreEqual(0, (int)GamePadType.Unknown);
            Assert.AreEqual(1, (int)GamePadType.GamePad);
            Assert.AreEqual(2, (int)GamePadType.Wheel);
            Assert.AreEqual(3, (int)GamePadType.ArcadeStick);
            Assert.AreEqual(4, (int)GamePadType.FlightStick);
            Assert.AreEqual(5, (int)GamePadType.DancePad);
            Assert.AreEqual(6, (int)GamePadType.Guitar);
            Assert.AreEqual(7, (int)GamePadType.AlternateGuitar);
            Assert.AreEqual(8, (int)GamePadType.DrumKit);
            Assert.AreEqual(768, (int)GamePadType.BigButtonPad);
        }

        [Test]
        public void KeysEnum()
        {
            Assert.AreEqual(0, (int)Key.None);
            Assert.AreEqual(8, (int)Key.Back);
            Assert.AreEqual(9, (int)Key.Tab);
            Assert.AreEqual(13, (int)Key.Enter);
            Assert.AreEqual(19, (int)Key.Pause);
            Assert.AreEqual(20, (int)Key.CapsLock);
            Assert.AreEqual(21, (int)Key.Kana);
            Assert.AreEqual(25, (int)Key.Kanji);
            Assert.AreEqual(27, (int)Key.Escape);
            Assert.AreEqual(28, (int)Key.ImeConvert);
            Assert.AreEqual(29, (int)Key.ImeNoConvert);
            Assert.AreEqual(32, (int)Key.Space);
            Assert.AreEqual(33, (int)Key.PageUp);
            Assert.AreEqual(34, (int)Key.PageDown);
            Assert.AreEqual(35, (int)Key.End);
            Assert.AreEqual(36, (int)Key.Home);
            Assert.AreEqual(37, (int)Key.Left);
            Assert.AreEqual(38, (int)Key.Up);
            Assert.AreEqual(39, (int)Key.Right);
            Assert.AreEqual(40, (int)Key.Down);
            Assert.AreEqual(41, (int)Key.Select);
            Assert.AreEqual(42, (int)Key.Print);
            Assert.AreEqual(43, (int)Key.Execute);
            Assert.AreEqual(44, (int)Key.PrintScreen);
            Assert.AreEqual(45, (int)Key.Insert);
            Assert.AreEqual(46, (int)Key.Delete);
            Assert.AreEqual(47, (int)Key.Help);
            Assert.AreEqual(48, (int)Key.D0);
            Assert.AreEqual(49, (int)Key.D1);
            Assert.AreEqual(50, (int)Key.D2);
            Assert.AreEqual(51, (int)Key.D3);
            Assert.AreEqual(52, (int)Key.D4);
            Assert.AreEqual(53, (int)Key.D5);
            Assert.AreEqual(54, (int)Key.D6);
            Assert.AreEqual(55, (int)Key.D7);
            Assert.AreEqual(56, (int)Key.D8);
            Assert.AreEqual(57, (int)Key.D9);
            Assert.AreEqual(65, (int)Key.A);
            Assert.AreEqual(66, (int)Key.B);
            Assert.AreEqual(67, (int)Key.C);
            Assert.AreEqual(68, (int)Key.D);
            Assert.AreEqual(69, (int)Key.E);
            Assert.AreEqual(70, (int)Key.F);
            Assert.AreEqual(71, (int)Key.G);
            Assert.AreEqual(72, (int)Key.H);
            Assert.AreEqual(73, (int)Key.I);
            Assert.AreEqual(74, (int)Key.J);
            Assert.AreEqual(75, (int)Key.K);
            Assert.AreEqual(76, (int)Key.L);
            Assert.AreEqual(77, (int)Key.M);
            Assert.AreEqual(78, (int)Key.N);
            Assert.AreEqual(79, (int)Key.O);
            Assert.AreEqual(80, (int)Key.P);
            Assert.AreEqual(81, (int)Key.Q);
            Assert.AreEqual(82, (int)Key.R);
            Assert.AreEqual(83, (int)Key.S);
            Assert.AreEqual(84, (int)Key.T);
            Assert.AreEqual(85, (int)Key.U);
            Assert.AreEqual(86, (int)Key.V);
            Assert.AreEqual(87, (int)Key.W);
            Assert.AreEqual(88, (int)Key.X);
            Assert.AreEqual(89, (int)Key.Y);
            Assert.AreEqual(90, (int)Key.Z);
            Assert.AreEqual(91, (int)Key.LeftWindows);
            Assert.AreEqual(92, (int)Key.RightWindows);
            Assert.AreEqual(93, (int)Key.Apps);
            Assert.AreEqual(95, (int)Key.Sleep);
            Assert.AreEqual(96, (int)Key.NumPad0);
            Assert.AreEqual(97, (int)Key.NumPad1);
            Assert.AreEqual(98, (int)Key.NumPad2);
            Assert.AreEqual(99, (int)Key.NumPad3);
            Assert.AreEqual(100, (int)Key.NumPad4);
            Assert.AreEqual(101, (int)Key.NumPad5);
            Assert.AreEqual(102, (int)Key.NumPad6);
            Assert.AreEqual(103, (int)Key.NumPad7);
            Assert.AreEqual(104, (int)Key.NumPad8);
            Assert.AreEqual(105, (int)Key.NumPad9);
            Assert.AreEqual(106, (int)Key.Multiply);
            Assert.AreEqual(107, (int)Key.Add);
            Assert.AreEqual(108, (int)Key.Separator);
            Assert.AreEqual(109, (int)Key.Subtract);
            Assert.AreEqual(110, (int)Key.Decimal);
            Assert.AreEqual(111, (int)Key.Divide);
            Assert.AreEqual(112, (int)Key.F1);
            Assert.AreEqual(113, (int)Key.F2);
            Assert.AreEqual(114, (int)Key.F3);
            Assert.AreEqual(115, (int)Key.F4);
            Assert.AreEqual(116, (int)Key.F5);
            Assert.AreEqual(117, (int)Key.F6);
            Assert.AreEqual(118, (int)Key.F7);
            Assert.AreEqual(119, (int)Key.F8);
            Assert.AreEqual(120, (int)Key.F9);
            Assert.AreEqual(121, (int)Key.F10);
            Assert.AreEqual(122, (int)Key.F11);
            Assert.AreEqual(123, (int)Key.F12);
            Assert.AreEqual(124, (int)Key.F13);
            Assert.AreEqual(125, (int)Key.F14);
            Assert.AreEqual(126, (int)Key.F15);
            Assert.AreEqual(127, (int)Key.F16);
            Assert.AreEqual(128, (int)Key.F17);
            Assert.AreEqual(129, (int)Key.F18);
            Assert.AreEqual(130, (int)Key.F19);
            Assert.AreEqual(131, (int)Key.F20);
            Assert.AreEqual(132, (int)Key.F21);
            Assert.AreEqual(133, (int)Key.F22);
            Assert.AreEqual(134, (int)Key.F23);
            Assert.AreEqual(135, (int)Key.F24);
            Assert.AreEqual(144, (int)Key.NumLock);
            Assert.AreEqual(145, (int)Key.Scroll);
            Assert.AreEqual(160, (int)Key.LeftShift);
            Assert.AreEqual(161, (int)Key.RightShift);
            Assert.AreEqual(162, (int)Key.LeftControl);
            Assert.AreEqual(163, (int)Key.RightControl);
            Assert.AreEqual(164, (int)Key.LeftAlt);
            Assert.AreEqual(165, (int)Key.RightAlt);
            Assert.AreEqual(166, (int)Key.BrowserBack);
            Assert.AreEqual(167, (int)Key.BrowserForward);
            Assert.AreEqual(168, (int)Key.BrowserRefresh);
            Assert.AreEqual(169, (int)Key.BrowserStop);
            Assert.AreEqual(170, (int)Key.BrowserSearch);
            Assert.AreEqual(171, (int)Key.BrowserFavorites);
            Assert.AreEqual(172, (int)Key.BrowserHome);
            Assert.AreEqual(173, (int)Key.VolumeMute);
            Assert.AreEqual(174, (int)Key.VolumeDown);
            Assert.AreEqual(175, (int)Key.VolumeUp);
            Assert.AreEqual(176, (int)Key.MediaNextTrack);
            Assert.AreEqual(177, (int)Key.MediaPreviousTrack);
            Assert.AreEqual(178, (int)Key.MediaStop);
            Assert.AreEqual(179, (int)Key.MediaPlayPause);
            Assert.AreEqual(180, (int)Key.LaunchMail);
            Assert.AreEqual(181, (int)Key.SelectMedia);
            Assert.AreEqual(182, (int)Key.LaunchApplication1);
            Assert.AreEqual(183, (int)Key.LaunchApplication2);
            Assert.AreEqual(186, (int)Key.OemSemicolon);
            Assert.AreEqual(187, (int)Key.OemPlus);
            Assert.AreEqual(188, (int)Key.OemComma);
            Assert.AreEqual(189, (int)Key.OemMinus);
            Assert.AreEqual(190, (int)Key.OemPeriod);
            Assert.AreEqual(191, (int)Key.OemQuestion);
            Assert.AreEqual(192, (int)Key.OemTilde);
            Assert.AreEqual(202, (int)Key.ChatPadGreen);
            Assert.AreEqual(203, (int)Key.ChatPadOrange);
            Assert.AreEqual(219, (int)Key.OemOpenBrackets);
            Assert.AreEqual(220, (int)Key.OemPipe);
            Assert.AreEqual(221, (int)Key.OemCloseBrackets);
            Assert.AreEqual(222, (int)Key.OemQuotes);
            Assert.AreEqual(223, (int)Key.Oem8);
            Assert.AreEqual(226, (int)Key.OemBackslash);
            Assert.AreEqual(229, (int)Key.ProcessKey);
            Assert.AreEqual(242, (int)Key.OemCopy);
            Assert.AreEqual(243, (int)Key.OemAuto);
            Assert.AreEqual(244, (int)Key.OemEnlW);
            Assert.AreEqual(246, (int)Key.Attn);
            Assert.AreEqual(247, (int)Key.Crsel);
            Assert.AreEqual(248, (int)Key.Exsel);
            Assert.AreEqual(249, (int)Key.EraseEof);
            Assert.AreEqual(250, (int)Key.Play);
            Assert.AreEqual(251, (int)Key.Zoom);
            Assert.AreEqual(253, (int)Key.Pa1);
            Assert.AreEqual(254, (int)Key.OemClear);
        }

        #endregion
    }
}
