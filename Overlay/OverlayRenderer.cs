using System;
using System.Collections.Generic;
using System.Numerics;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SelectXYZ_Cheat.Models;
using SelectXYZ_Cheat.ESP;
using D2D1 = SharpDX.Direct2D1;
using DWrite = SharpDX.DirectWrite;

namespace SelectXYZ_Cheat.Overlay
{
    public class OverlayRenderer : IDisposable
    {
        private D2D1.Factory d2dFactory;
        private DWrite.Factory writeFactory;
        private RenderTarget renderTarget;
        private Dictionary<string, SolidColorBrush> brushes;
        private Dictionary<string, TextFormat> textFormats;
        private bool disposed = false;

        public OverlayRenderer(IntPtr hwnd)
        {
            InitializeDirectX(hwnd);
            InitializeBrushes();
            InitializeTextFormats();
        }

        private void InitializeDirectX(IntPtr hwnd)
        {
            d2dFactory = new D2D1.Factory();
            writeFactory = new DWrite.Factory();

            var hwndRenderTargetProperties = new HwndRenderTargetProperties()
            {
                Hwnd = hwnd,
                PixelSize = new Size2(1920, 1080),
                PresentOptions = PresentOptions.None
            };

            var renderTargetProperties = new RenderTargetProperties()
            {
                Type = RenderTargetType.Hardware,
                PixelFormat = new PixelFormat(Format.B8G8R8A8_UNorm, D2D1.AlphaMode.Premultiplied)
            };

            renderTarget = new HwndRenderTarget(d2dFactory, renderTargetProperties, hwndRenderTargetProperties);
        }

        private void InitializeBrushes()
        {
            brushes = new Dictionary<string, SolidColorBrush>
            {
                ["White"] = new SolidColorBrush(renderTarget, Color.White),
                ["Black"] = new SolidColorBrush(renderTarget, Color.Black),
                ["Red"] = new SolidColorBrush(renderTarget, Color.Red),
                ["Green"] = new SolidColorBrush(renderTarget, Color.Green),
                ["Blue"] = new SolidColorBrush(renderTarget, Color.Blue),
                ["Yellow"] = new SolidColorBrush(renderTarget, Color.Yellow),
                ["Cyan"] = new SolidColorBrush(renderTarget, Color.Cyan),
                ["Magenta"] = new SolidColorBrush(renderTarget, Color.Magenta),
                ["Purple"] = new SolidColorBrush(renderTarget, new Color(110, 69, 226)),
                ["Orange"] = new SolidColorBrush(renderTarget, Color.Orange)
            };
        }

        private void InitializeTextFormats()
        {
            textFormats = new Dictionary<string, TextFormat>
            {
                ["Default"] = new TextFormat(writeFactory, "Segoe UI", 12),
                ["Small"] = new TextFormat(writeFactory, "Segoe UI", 10),
                ["Large"] = new TextFormat(writeFactory, "Segoe UI", 14)
            };
        }

        public void Render()
        {
            if (disposed) return;

            renderTarget.BeginDraw();
            renderTarget.Clear(Color.Transparent);

            var players = ESPManager.Instance.GetPlayers();
            var settings = ESPManager.Instance.Settings;

            foreach (var player in players)
            {
                if (player.IsLocalPlayer || !player.IsAlive) continue;

                RenderPlayer(player, settings);
            }

            renderTarget.EndDraw();
        }

        private void RenderPlayer(Player player, ESPSettings settings)
        {
            // Render 2D/3D Box
            if (settings.BoxType != ESPBoxType.None)
            {
                RenderBox(player, settings);
            }

            // Render Skeleton
            if (settings.SkeletonESP)
            {
                RenderSkeleton(player, settings);
            }

            // Render Health Bar
            if (settings.HealthBar)
            {
                RenderHealthBar(player, settings);
            }

            // Render Player Name
            if (settings.PlayerName)
            {
                RenderPlayerName(player, settings);
            }

            // Render Distance
            if (settings.DistanceESP)
            {
                RenderDistance(player, settings);
            }

            // Render Head Circle
            if (settings.HeadCircle)
            {
                RenderHeadCircle(player, settings);
            }
        }

        private void RenderBox(Player player, ESPSettings settings)
        {
            var brush = GetBrush(settings.BoxColor);
            var outlineBrush = GetBrush(settings.OutlineColor);
            var box = player.BoundingBox;

            var rect = new RectangleF(box.Left, box.Top, box.Width, box.Height);

            switch (settings.BoxType)
            {
                case ESPBoxType.Box2D:
                    // Fill
                    if (settings.BoxFill)
                    {
                        var fillBrush = GetBrush(settings.BoxFillColor);
                        fillBrush.Opacity = 0.3f;
                        renderTarget.FillRectangle(rect, fillBrush);
                        fillBrush.Opacity = 1.0f;
                    }
                    // Outline
                    renderTarget.DrawRectangle(rect, outlineBrush, 2);
                    break;

                case ESPBoxType.CornerBox:
                    RenderCornerBox(box, GetBrush(settings.CornerBoxColor));
                    break;

                case ESPBoxType.Box3D:
                    Render3DBox(player, GetBrush(settings.Box3DColor));
                    break;
            }
        }

        private void RenderCornerBox(BoundingBox2D box, SolidColorBrush brush)
        {
            float cornerSize = Math.Min(box.Width, box.Height) * 0.2f;

            // Top-left corner
            renderTarget.DrawLine(new Vector2(box.Left, box.Top), new Vector2(box.Left + cornerSize, box.Top), brush, 2);
            renderTarget.DrawLine(new Vector2(box.Left, box.Top), new Vector2(box.Left, box.Top + cornerSize), brush, 2);

            // Top-right corner
            renderTarget.DrawLine(new Vector2(box.Right, box.Top), new Vector2(box.Right - cornerSize, box.Top), brush, 2);
            renderTarget.DrawLine(new Vector2(box.Right, box.Top), new Vector2(box.Right, box.Top + cornerSize), brush, 2);

            // Bottom-left corner
            renderTarget.DrawLine(new Vector2(box.Left, box.Bottom), new Vector2(box.Left + cornerSize, box.Bottom), brush, 2);
            renderTarget.DrawLine(new Vector2(box.Left, box.Bottom), new Vector2(box.Left, box.Bottom - cornerSize), brush, 2);

            // Bottom-right corner
            renderTarget.DrawLine(new Vector2(box.Right, box.Bottom), new Vector2(box.Right - cornerSize, box.Bottom), brush, 2);
            renderTarget.DrawLine(new Vector2(box.Right, box.Bottom), new Vector2(box.Right, box.Bottom - cornerSize), brush, 2);
        }

        private void Render3DBox(Player player, SolidColorBrush brush)
        {
            // Simple 3D box representation - would need proper 3D to 2D projection
            var box = player.BoundingBox;
            float depth = box.Width * 0.3f;

            // Front face
            var frontRect = new RectangleF(box.Left, box.Top, box.Width, box.Height);
            renderTarget.DrawRectangle(frontRect, brush, 2);

            // Back face (offset)
            var backRect = new RectangleF(box.Left - depth, box.Top - depth, box.Width, box.Height);
            renderTarget.DrawRectangle(backRect, brush, 1);

            // Connect corners
            renderTarget.DrawLine(new Vector2(box.Left, box.Top), new Vector2(box.Left - depth, box.Top - depth), brush, 1);
            renderTarget.DrawLine(new Vector2(box.Right, box.Top), new Vector2(box.Right - depth, box.Top - depth), brush, 1);
            renderTarget.DrawLine(new Vector2(box.Left, box.Bottom), new Vector2(box.Left - depth, box.Bottom - depth), brush, 1);
            renderTarget.DrawLine(new Vector2(box.Right, box.Bottom), new Vector2(box.Right - depth, box.Bottom - depth), brush, 1);
        }

        private void RenderSkeleton(Player player, ESPSettings settings)
        {
            var brush = GetBrush(settings.SkeletonColor);
            var box = player.BoundingBox;

            // Simple skeleton - would need bone positions for proper skeleton
            float centerX = box.Center.X;
            float headY = box.Top;
            float neckY = box.Top + box.Height * 0.15f;
            float torsoY = box.Top + box.Height * 0.6f;
            float bottomY = box.Bottom;

            // Head to neck
            renderTarget.DrawLine(new Vector2(centerX, headY), new Vector2(centerX, neckY), brush, 2);
            
            // Torso
            renderTarget.DrawLine(new Vector2(centerX, neckY), new Vector2(centerX, torsoY), brush, 2);
            
            // Arms
            float armSpan = box.Width * 0.8f;
            renderTarget.DrawLine(new Vector2(centerX - armSpan/2, neckY + box.Height * 0.1f), 
                                new Vector2(centerX + armSpan/2, neckY + box.Height * 0.1f), brush, 2);
            
            // Legs
            renderTarget.DrawLine(new Vector2(centerX, torsoY), new Vector2(centerX - box.Width * 0.2f, bottomY), brush, 2);
            renderTarget.DrawLine(new Vector2(centerX, torsoY), new Vector2(centerX + box.Width * 0.2f, bottomY), brush, 2);
        }

        private void RenderHealthBar(Player player, ESPSettings settings)
        {
            var healthBrush = GetBrush(settings.HealthBarColor);
            var backgroundBrush = GetBrush("Black");
            var box = player.BoundingBox;

            float barWidth = 4;
            float barHeight = box.Height;
            float barX = box.Left - barWidth - 5;
            float barY = box.Top;

            // Background
            var bgRect = new RectangleF(barX, barY, barWidth, barHeight);
            renderTarget.FillRectangle(bgRect, backgroundBrush);

            // Health
            float healthHeight = barHeight * player.HealthPercentage;
            var healthRect = new RectangleF(barX, barY + barHeight - healthHeight, barWidth, healthHeight);
            renderTarget.FillRectangle(healthRect, healthBrush);

            // Border
            renderTarget.DrawRectangle(bgRect, GetBrush("White"), 1);

            // Health number
            if (settings.HealthNumber)
            {
                string healthText = $"{(int)player.Health}";
                var textRect = new RectangleF(barX - 30, barY + barHeight/2 - 6, 25, 12);
                renderTarget.DrawText(healthText, textFormats["Small"], textRect, GetBrush("White"));
            }
        }

        private void RenderPlayerName(Player player, ESPSettings settings)
        {
            var brush = GetBrush("White");
            var box = player.BoundingBox;
            
            var textRect = new RectangleF(box.Left, box.Top - 20, box.Width, 15);
            renderTarget.DrawText(player.Name, textFormats["Default"], textRect, brush, DrawTextOptions.None, MeasuringMode.Natural);
        }

        private void RenderDistance(Player player, ESPSettings settings)
        {
            var brush = GetBrush("White");
            var box = player.BoundingBox;
            
            string distanceText = $"{(int)player.Distance}m";
            var textRect = new RectangleF(box.Left, box.Bottom + 5, box.Width, 15);
            renderTarget.DrawText(distanceText, textFormats["Small"], textRect, brush);
        }

        private void RenderHeadCircle(Player player, ESPSettings settings)
        {
            var brush = GetBrush(settings.HeadCircleColor);
            var headPos = player.HeadScreenPosition;
            
            float radius = player.BoundingBox.Width * 0.15f;
            var ellipse = new Ellipse(new Vector2(headPos.X, headPos.Y), radius, radius);
            renderTarget.DrawEllipse(ellipse, brush, 2);
        }

        private SolidColorBrush GetBrush(string colorName)
        {
            return brushes.ContainsKey(colorName) ? brushes[colorName] : brushes["White"];
        }

        public void Dispose()
        {
            if (disposed) return;

            foreach (var brush in brushes.Values)
                brush?.Dispose();
            
            foreach (var format in textFormats.Values)
                format?.Dispose();

            renderTarget?.Dispose();
            writeFactory?.Dispose();
            d2dFactory?.Dispose();

            disposed = true;
        }
    }
}