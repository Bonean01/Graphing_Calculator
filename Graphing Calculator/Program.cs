//using System.Collections.Generic;
//using System.Data;
//using System.Security.Cryptography;
//using IronPython;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using static SDL2.SDL;
using static Input.Input;
using static SDL2.SDL_ttf;
using GUI;
//using static System.Net.Mime.MediaTypeNames;
//using IronPython.Modules;

class Program
{
    // Program variables and constants.
    public static IntPtr window;
    public static IntPtr renderer;

    public static dynamic? pyProgram;

    public static int page = 0;
    public static int process;
    public static bool running = true;
    public static int mouseX = 0;
    public static int mouseY = 0;

    public static int tileSize = 10;
    public static int zoom = 50;
    public static bool showText = true;
    public static bool showGrid = true;
    public static bool mouseDown = false;
    public static byte[] borderColor = new byte[] { 60, 60, 60, 255 };
    public static byte[] backgroundColor = new byte[] { 30, 30, 30, 255 };
    public static byte[] textColor = new byte[] { 255, 255, 255, 255 };
    public static byte[]? gridColor;
    public static byte[]? sliderBaseColor;
    public static int pressCooldown = 0;

    public const int width = 1000;
    public const int height = 600;
    public const int minZoom = 3;
    public const int maxZoom = 400;

    // Text variables declaration.
    public static IntPtr font;
    public static SDL_Color txtColor;
    public static IntPtr txtSurface;
    public static IntPtr txtTexture;
    public static IntPtr speedTexture;
    public static SDL_Rect speedRect;
    public static IntPtr zoomTexture;
    public static SDL_Rect zoomRect;

    // Instantiating other classes.
    public static Camera camera = new(-103, 56, 0);
    public static DownBar downBar = new(0, height - height / 8, width, height / 8, borderColor);
    public static Header header = new(0, 0, width, 30, borderColor);
    public static SideBar sideBar = new(0, 0, 30, height, borderColor);
    public static FormulaManager formulaManager = new();
    public static BorderCover? borderCover;
    public static Graph? graph;
    public static FormulaRenderer? formulaRenderer;

    public static CalculatorButton[] calculatorButtons = new CalculatorButton[10];

    public static FormulasBtn formulasBtn = new(800, 538, 170, 50, new byte[] { 255, 0, 0, 255 }, 1 /*formulas*/);
    public static BackBtn backBtn = new(800, 538, 170, 50, new byte[] {255, 0, 0, 255 }, 0 /*main*/);
    //public static CoordsDisplay? coordsDisplay;
    public static SpeedSlider? speedSlider;
    public static ZoomSlider? zoomSlider;

    static void Main(string[] args)
    {
        // Main loop.
        Setup();
        while (running)
          { Thread.Sleep(1); // time in ms
            if ((process = PollEvents()) != 0)
            { Render(); }
        } CleanUp(); }

    /// <summary>
    /// Creates the window and initializes text and rectangle variables
    /// </summary>
    private static void Setup()
    {
        if (SDL_Init(SDL_INIT_VIDEO) < 0)
        { Console.WriteLine($"There was an error initializing SDL. {SDL_GetError()}"); }

        if (TTF_Init() < 0)
        { Console.WriteLine($"There was an issue inizializing TTF. {SDL_GetError()}"); }


        window = SDL_CreateWindow("Graphing Calculator", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, width, height,
            SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
        if (window == IntPtr.Zero) { Console.WriteLine($"There was an error creating the window {SDL_GetError()}"); }

        renderer = SDL_CreateRenderer(window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
        if (renderer == IntPtr.Zero) { Console.WriteLine($"There was an error creating the renderer {SDL_GetError()}"); }

        font = TTF_OpenFont("arial.ttf", 200);
;
        ScriptRuntime py = Python.CreateRuntime();
        pyProgram = py.UseFile("../../../PointCalculator.py");
        Initializer();
    }

    public static void Initializer()
    {
        switch (page)
        {
            case 0: // main
                // Initializing variables for Main page.
                gridColor = new byte[] { 100, 100, 100, 255 };
                sliderBaseColor = new byte[] { 100, 100, 100, 255 };

                // Instantiating classes for Main page.
                borderCover = new(0, 0, 30, 30, borderColor);
                graph = new(camera);
                speedSlider = new(400, 536, 200, 10, "Speed", sliderBaseColor, speedTexture, speedRect);
                zoomSlider = new(400, 576, 200, 10, "Zoom", sliderBaseColor, zoomTexture, zoomRect);
                formulaRenderer = new();
                camera.UpdateSpeed((float)tileSize);

                // Text variables initialization.
                txtColor = new SDL_Color();
                txtColor.r = textColor[0]; txtColor.g = textColor[1]; txtColor.b = textColor[2]; txtColor.a = textColor[3];
                Render();
                break;

            case 1: // formulas
                int vertOffset = 70;
                for (int i = 0; i <= 2; i++)
                {
                    calculatorButtons[i] = new CalculatorButton(70 * i + 30, vertOffset, 50, 50, new byte[] { 255, 0, 0, 255 }, $"{i+7}");
                    calculatorButtons[i+3] = new CalculatorButton(70 * i + 30, vertOffset + 70, 50, 50, new byte[] { 255, 0, 0, 255 }, $"{i+4}");
                    calculatorButtons[i+6] = new CalculatorButton(70 * i + 30, vertOffset + 140, 50, 50, new byte[] { 255, 0, 0, 255 }, $"{i+1}");
                }
                calculatorButtons[9] = new CalculatorButton(30, vertOffset + 210, 50, 50, new byte[] { 255, 0, 0, 255 }, "0");
                break;
        }
    }

    /// <summary>
    /// Handles Poll Events such as keyboard or mouse input and sorts them for optimizing.
    /// </summary>
    static int PollEvents()
    {
        while (SDL_PollEvent(out SDL_Event events) == 1)
        {
            switch (events.type)
            {
                case SDL_EventType.SDL_QUIT:
                    running = false;
                    return 1;

                case SDL_EventType.SDL_KEYDOWN:
                    KeyboardInput(events.key.keysym.sym.ToString(), camera);
                    return 1;

                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    mouseDown = true;
                    ClickManager(events.motion.x, events.motion.y);
                    return 2;

                case SDL_EventType.SDL_MOUSEBUTTONUP:
                    mouseDown = false;
                    return 2;

                case SDL_EventType.SDL_MOUSEMOTION:
                    ClickManager(events.motion.x, events.motion.y);
                    return 2;
            }
        }
        if (pressCooldown > 0) { pressCooldown--; }
        return 0;
    }

    /// <summary>
    /// Checks wether a button has been clicked or not.
    /// </summary>
    static void ClickManager(int mouseX, int mouseY) // Include here all buttons inside a page.
    {
        if (pressCooldown == 0)
        {
            switch (page)
            {
                case 0:
                    formulasBtn.CheckClick(mouseX, mouseY, mouseDown);
                    break;
                case 1:
                    backBtn.CheckClick(mouseX, mouseY, mouseDown);

                    foreach (CalculatorButton button in calculatorButtons)
                    {
                        button.CheckClick(mouseX, mouseY, mouseDown);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// This methods takes care of rendering to the screen and clearing it.
    /// </summary>
    static void Render()
    {
        SDL_SetRenderDrawColor(renderer, backgroundColor[0], backgroundColor[1], backgroundColor[2], backgroundColor[3]);
        SDL_RenderClear(renderer);
        switch (page)
        {
            case 0: // main
                #pragma warning disable CS8604
                #pragma warning disable CS8602
                header.Render(renderer);
                sideBar.Render(renderer);
                graph.GridDrawer(gridColor);
                formulaRenderer.Render(renderer);
                borderCover.Render(renderer);
                downBar.Render(renderer);
                formulasBtn.Render(renderer);
                //coordsDisplay.Render(renderer, camera.getXCoord(), camera.getYCoord());
                speedSlider.Render(renderer);
                zoomSlider.Render(renderer);
                #pragma warning restore CS8604
                #pragma warning restore CS8602
                break;

            case 1: // formulas
                downBar.Render(renderer);
                backBtn.Render(renderer);
                foreach (CalculatorButton button in calculatorButtons)
                {
                    button.Render(renderer);
                }
                break;
        }
        SDL_RenderPresent(renderer);
    }

    /// <summary>
    /// This method deletes the program, text and rectangle resources created previously.
    /// </summary>
    public static void CleanUp()
    {
        switch(page)
        {
            case 0: // main
                // Clearing text resources from Main page.
                break;
        }
        GC.Collect();

        if (!running)
        {
            // Clearing program resources.
            TTF_CloseFont(font);
            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);
            TTF_Quit();
            SDL_Quit();
        }
    }
}

/*NOTES

 OPTIMIZATIONS
 ------------
 * Delete the resources created for a page when changed, then initialize them again if it is recalled, maybe create a loading screen in another thread if the loading time is too high.
 
REVISIONS
-------------
 * Check the coordinate asination of vertical lines when zooming and the slight inaccuracy when hitting CamCoords that end in 3.

  IMPORTANT
-------------
 When done, remember to change the output type to: "windows application" in the Project/Graphing Calculator Properties window.
*/