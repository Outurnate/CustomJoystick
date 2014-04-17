using InputManager;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace InputSim
{
    class ArduinoInterface
    {
        [Flags]
        enum Buttons
        {
            None = 0,
            LeftClick = 1,
            RightClick = 2,
            Crouch = 4,
            W = 8,
            A = 16,
            S = 32,
            D = 64
        }

        public float MouseSensitivity
        {
            set;
            get;
        }

        public Keys Crouch
        {
            set;
            get;
        }

        public Keys W
        {
            set;
            get;
        }

        public Keys A
        {
            set;
            get;
        }

        public Keys S
        {
            set;
            get;
        }

        public Keys D
        {
            set;
            get;
        }

        private SerialPort port;
        private bool pleftClick = false;
        private bool prightClick = false;
        private bool pcrouch = false;
        private bool pW = false;
        private bool pA = false;
        private bool pS = false;
        private bool pD = false;
        private Thread pumpThread;
        private bool run = true;

        public ArduinoInterface(string port)
        {
            this.port = new SerialPort(port, 9600);
            this.port.Open();
            this.Crouch = Keys.Shift;
            this.W = Keys.W;
            this.A = Keys.A;
            this.S = Keys.S;
            this.D = Keys.D;
            this.MouseSensitivity = 1.0f;
            pumpThread = new Thread(new ThreadStart(this.pumpData));
            pumpThread.Start();
            this.run = true;
        }

        private void pumpData()
        {
            while (run)
            {
                string[] parts = port.ReadLine().Split(',');
                if (parts.Length == 3)
                {
                    float xvel = 0.0f, yvel = 0.0f;
                    if (!float.TryParse(parts[0], out xvel) && !float.TryParse(parts[1], out yvel))
                        return;
                    // [unused] [d] [s] [a] [w] [crouch] [rightclick] [leftclick]
                    Buttons but = (Buttons)Convert.ToInt32(parts[2].Replace("\r", ""), 16);
                    bool leftClick = (but & Buttons.LeftClick) != Buttons.None;
                    bool rightClick = (but & Buttons.RightClick) != Buttons.None;
                    bool crouch = (but & Buttons.Crouch) != Buttons.None;
                    bool W = (but & Buttons.W) != Buttons.None;
                    bool A = (but & Buttons.A) != Buttons.None;
                    bool S = (but & Buttons.S) != Buttons.None;
                    bool D = (but & Buttons.D) != Buttons.None;

                    Mouse.MoveRelative((int)(xvel * MouseSensitivity), (int)(yvel * MouseSensitivity));

                    if (leftClick && !pleftClick)
                        Mouse.ButtonDown(Mouse.MouseKeys.Left);
                    if (!leftClick && pleftClick)
                        Mouse.ButtonUp(Mouse.MouseKeys.Left);

                    if (rightClick && !prightClick)
                        Mouse.ButtonDown(Mouse.MouseKeys.Right);
                    if (!rightClick && prightClick)
                        Mouse.ButtonUp(Mouse.MouseKeys.Right);

                    if (crouch && !pcrouch)
                        Keyboard.KeyDown(this.Crouch);
                    if (!crouch && pcrouch)
                        Keyboard.KeyUp(this.Crouch);

                    if (W && !pW)
                        Keyboard.KeyDown(this.W);
                    if (!W && pW)
                        Keyboard.KeyUp(this.W);

                    if (A && !pA)
                        Keyboard.KeyDown(this.A);
                    if (!A && pA)
                        Keyboard.KeyUp(this.A);

                    if (S && !pS)
                        Keyboard.KeyDown(this.S);
                    if (!S && pS)
                        Keyboard.KeyUp(this.S);

                    if (D && !pD)
                        Keyboard.KeyDown(this.D);
                    if (!D && pD)
                        Keyboard.KeyUp(this.D);

                    pleftClick = leftClick;
                    prightClick = rightClick;
                    pcrouch = crouch;
                    pW = W;
                    pA = A;
                    pS = S;
                    pD = D;
                }
            }
        }

        public void Close()
        {
            run = false;
            port.Close();
            pumpThread.Join();
        }
    }
}
