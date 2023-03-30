namespace Input {
    public class Input
    {
        public static bool[] mouseBtns = new bool[] {false, false, false};

        public static void KeyboardInput(string key, Camera camera)
        {
            switch (key)
            {
                case "SDLK_UP":
                    camera.MoveUp(); break;

                case "SDLK_DOWN":
                    camera.MoveDown(); break;

                case "SDLK_RIGHT":
                    camera.MoveRight(); break;

                case "SDLK_LEFT":
                    camera.MoveLeft();  break;

                case "SDLK_RETURN":
                    if (Program.zoom > Program.minZoom) { Program.zoom -= 1; } break;

                case "SDLK_SPACE":
                    if (Program.zoom < Program.maxZoom) { Program.zoom += 1; } break;
            }
        }
    }
}
