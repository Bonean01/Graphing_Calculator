using static SDL2.SDL;
using static SDL2.SDL_ttf;
using static Program;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace GUI
{
    public class Box
    {
        int xCoord;
        int yCoord;
        int width;
        public int height;
        byte[] color;
        SDL_Rect rect;
        public Box(int xCoord, int yCoord, int width, int height, byte[] color)
        {
            this.xCoord = xCoord;
            this.yCoord = yCoord;
            this.width = width;
            this.height = height;
            this.color = color;
            this.rect = new SDL_Rect
            { x = xCoord,
                y = yCoord,
                w = width,
                h = height };
        }
        // This method will be pased on to the render funciton in the main file.
        public void Render(IntPtr renderer)
        {
            SDL_SetRenderDrawColor(renderer, color[0], color[1], color[2], color[3]);
            SDL_RenderFillRect(renderer, ref rect);
        }
    }

    public class PageButton
    {
        public int xCoord;
        public int yCoord;
        public int width;
        public int height;
        public byte[] color;
        public int pageChanger;
        SDL_Rect rect;

        public PageButton(int xCoord, int yCoord, int width, int height, byte[] color, int pageChanger)
        {
            this.xCoord = xCoord;
            this.yCoord = yCoord;
            this.width = width;
            this.height = height;
            this.color = color;
            this.pageChanger = pageChanger;
            this.rect = new SDL_Rect
            { x = xCoord,
                y = yCoord,
                w = width,
                h = height };
        }

        public void Render(IntPtr renderer)
        {
            SDL_SetRenderDrawColor(renderer, color[0], color[1], color[2], color[3]);
            SDL_RenderFillRect(renderer, ref rect);
        }

        public bool CheckClick(int mouseX, int mouseY, bool mouseDown)
        {
            if (mouseX < xCoord + this.width && mouseX > xCoord && mouseY < yCoord + this.height && mouseY > yCoord)
            {
                color = new byte[] { 0, 255, 0, 255 };
                if (mouseDown)
                {
                    CleanUp();
                    page = pageChanger;
                    pressCooldown += 10;
                    Initializer();
                }
                return true;
            } else { color = new byte[] { 255, 0, 0, 255 }; return false; }
        }
    }

    public class CalculatorButton
    {
        public int xCoord;
        public int yCoord;
        public int width;
        public int height;
        public byte[] color;
        public string name;
        SDL_Rect rect;

        public CalculatorButton(int xCoord, int yCoord, int width, int height, byte[] color, string name)
        {
            this.xCoord = xCoord;
            this.yCoord = yCoord;
            this.width = width;
            this.height = height;
            this.color = color;
            this.name = name;
            this.rect = new SDL_Rect
            {
                x = xCoord,
                y = yCoord,
                w = width,
                h = height
            };
        }

        public void Render(IntPtr renderer)
        {
            SDL_SetRenderDrawColor(renderer, color[0], color[1], color[2], color[3]);
            SDL_RenderFillRect(renderer, ref rect);
        }

        public void CheckClick(int mouseX, int mouseY, bool mouseDown)
        {
            if (mouseX < xCoord + this.width && mouseX > xCoord && mouseY < yCoord + this.height && mouseY > yCoord)
            {
                color = new byte[] { 0, 255, 0, 255 };
                if (mouseDown)
                {
                    formulaManager.FormulaProcessor(this.name);
                    pressCooldown += 10;
                    Console.WriteLine(formulaManager.formula);
                }
            }
            else { color = new byte[] { 255, 0, 0, 255 }; }
        }
    }

    public class Slider
    {
        int x;
        int y;
        int hitboxWidth;
        int hitboxHeight;
        string name;
        byte[] color;
        IntPtr texture;
        SDL_Rect dstRect;
        SDL_Rect SliderBar;
        public Slider(int xCoord, int yCoord, int hitboxWidth, int hitboxHeight, string name, byte[] color, IntPtr texture, SDL_Rect dstRect)
        {
            this.x = xCoord;
            this.y = yCoord;
            this.hitboxWidth = hitboxWidth;
            this.hitboxHeight = hitboxHeight;
            this.name = name;
            this.color = color;
            this.dstRect = dstRect;
            this.SliderBar = new SDL_Rect
            { x = xCoord,
                y = yCoord,
                w = hitboxWidth,
                h = hitboxHeight };
        }

        public void Render(IntPtr renderer)
        {
            SDL_RenderCopy(renderer, texture, (IntPtr)null, ref dstRect);
            SDL_SetRenderDrawColor(renderer, color[0], color[1], color[2], color[3]);
            SDL_RenderFillRect(renderer, ref SliderBar);
        }
    }

    public class DownBar : Box { public DownBar(int xCoord, int yCoord, int width, int height, byte[] color) : base(xCoord, yCoord, width, height, color) { } }
    public class Header : Box { public Header(int xCoord, int yCoord, int width, int height, byte[] color) : base(xCoord, yCoord, width, height, color) { } }
    public class SideBar : Box { public SideBar(int xCoord, int yCoord, int width, int height, byte[] color) : base(xCoord, yCoord, width, height, color) { } }
    public class BorderCover : Box { public BorderCover(int xCoord, int yCoord, int width, int height, byte[] color) : base(xCoord, yCoord, width, height, color) { } }

    public class FormulasBtn : PageButton { public FormulasBtn(int xCoord, int yCoord, int width, int height, byte[] color, int pageChanger) : base(xCoord, yCoord, width, height, color, pageChanger) { } }
    public class BackBtn : PageButton { public BackBtn(int xCoord, int yCoord, int width, int height, byte[] color, int pageChanger) : base(xCoord, yCoord, width, height, color, pageChanger) { } }

    public class SpeedSlider : Slider { public SpeedSlider(int xCoord, int yCoord, int hitboxWidth, int hitboxHeight, string name, byte[] color, IntPtr texture, SDL_Rect rect) : base(xCoord, yCoord, hitboxWidth, hitboxHeight, name, color, texture, rect) { } }
    public class ZoomSlider : Slider { public ZoomSlider(int xCoord, int yCoord, int hitboxWidth, int hitboxHeight, string name, byte[] color, IntPtr texture, SDL_Rect rect) : base(xCoord, yCoord, hitboxWidth, hitboxHeight, name, color, texture, rect) { } }


    public class Graph
    {
        Camera camera;
        public Graph(Camera camera)
        {
            this.camera = camera;
        }
        public void GridDrawer(byte[] color)
        {
            float camX = camera.getXCoord();
            float camY = camera.getYCoord();
            double camXTileRate = camX / tileSize;
            double camYTileRate = camY / tileSize;
            double[] offset = new double[] { (camXTileRate - Math.Truncate(camXTileRate)) * zoom, (camYTileRate - Math.Truncate(camYTileRate)) * zoom };
            Console.WriteLine($"{camX}:{camY} Cam Coords");
            SDL_SetRenderDrawColor(renderer, color[0], color[1], color[2], color[3]);
            for (int i = -zoom; i <= width; i += zoom)
            {
                VertLine(i, offset[0], camX);
            }
            for (int j = height; j > -zoom; j -= zoom)
            {
                HorLine(j, offset[1], camY);
            }
        }

        public void VertLine(int i, double offset, float camX)
        {
            float x = camX + (i - (float)offset) * tileSize / zoom;
            if (i - offset > 30) { SDL_RenderDrawLine(renderer, i + (int)-offset, 30, i + (int)-offset, height); }
            string text = $"{Math.Round(x + 0.001f)}";
            int charNum = 0;
            foreach (char _ in text) { charNum += 1; }
            if (zoom > 30 && showText)
            { TextManager(i + (int)-offset - 4 * charNum, 10, text, charNum); }
        }

        public void HorLine(int j, double offset, float camY)
        {
            float y = camY - (j + (float)offset) * tileSize / zoom;
            if (j + offset > 30) { SDL_RenderDrawLine(renderer, 30, j + (int)offset, width, j + (int)offset); }
            string text = $"{Math.Round(y + 0.001f)}";
            int charNum = 0;
            foreach (char _ in text) { charNum += 1; }
            if (zoom > 30 && showText)
            { TextManager(5, j + (int)offset - 5, text, charNum); }
        }

        public static void TextManager(int x, int y, string text, int charNum)
        {
            txtSurface = TTF_RenderText_Solid(font, text, txtColor);
            txtTexture = SDL_CreateTextureFromSurface(renderer, txtSurface);

            SDL_Rect dstRect = new SDL_Rect
               {x = x,
                y = y,
                w = 7 * charNum,
                h = 10};

            SDL_RenderCopy(renderer, txtTexture, (IntPtr)null, ref dstRect);
            SDL_DestroyTexture(txtTexture);
            SDL_FreeSurface(txtSurface);
        }
    }

    public class FormulaRenderer
    {
        public FormulaRenderer() { }
        int[] previous = { 0, 0 };
        public void Render(IntPtr renderer)
        {
            SDL_SetRenderDrawColor(renderer, 90, 90, 255, 255);
            for (int pixel = 30; pixel <= width; pixel+=20)
            {
                SDL_RenderDrawLine(renderer, previous[0], previous[1], previous[0]=pixel, previous[1]=(int)pyProgram.calculate_point("x", pixel));
            }
        }
    }

    /*public class CoordsDisplay
    {
        public int x;
        public int y;
        public float camX;
        public float camY;
        public int charNumX;
        public int charNumY;
        public SDL_Rect rect;

        public CoordsDisplay(int x, int y, float camX, float camY)
        {
            this.x = x;
            this.y = y;
            this.camX = camX;
            this.camY = camY;
        }

        public void Render(IntPtr renderer, float camX, float camY)
        {
            SDL_RenderCopy(renderer, coordsTxtTexture, (IntPtr)null, ref coordsRect);
            SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
            SDL_RenderDrawRect(renderer, ref coordBoxRectX);
            SDL_RenderDrawRect(renderer, ref coordBoxRectY);
            txtSurface = TTF_RenderText_Solid(font, camX.ToString(), txtColor);
            txtTexture = SDL_CreateTextureFromSurface(renderer, txtSurface);
            charNumX = 0;
            foreach (char _ in camX.ToString()) { charNumX += 1; }
            charNumY = 0;
            foreach (char _ in camX.ToString()) { charNumY += 1; }
            SDL_Rect dstRect = new SDL_Rect
            {
                x = x + 103,
                y = y - 11,
                w = 10 * charNumX,
                h = 13
            };
            SDL_RenderCopy(renderer, txtTexture, (IntPtr)null, ref dstRect);
            SDL_DestroyTexture(txtTexture);
            SDL_FreeSurface(txtSurface);
            txtSurface = TTF_RenderText_Solid(font, camY.ToString(), txtColor);
            txtTexture = SDL_CreateTextureFromSurface(renderer, txtSurface);
            dstRect = new SDL_Rect
            {
                x = x,
                y = y,
                w = 10 * charNumY,
                h = 13
            };
            SDL_RenderCopy(renderer, txtTexture, (IntPtr)null, ref dstRect);
            SDL_DestroyTexture(txtTexture);
            SDL_FreeSurface(txtSurface);
        }
    }*/
}