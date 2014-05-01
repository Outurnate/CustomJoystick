using InputManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace InputSim
{
    class ArduinoInterface
    {
        public class KeyboardConfig
        {
            public KeyboardConfig()
            {
                this.Crouch = Keys.Shift;
                this.W = Keys.W;
                this.A = Keys.A;
                this.S = Keys.S;
                this.D = Keys.D;
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

            public void LoadConfig(Stream config)
            {
                StreamReader reader = new StreamReader(config);
                int iCrouch, iW, iA, iS, iD;
                try
                {
                    if (!int.TryParse(reader.ReadLine(), out iCrouch))
                        return;
                    Crouch = (Keys)iCrouch;
                    if (!int.TryParse(reader.ReadLine(), out iW))
                        return;
                    W = (Keys)iW;
                    if (!int.TryParse(reader.ReadLine(), out iA))
                        return;
                    A = (Keys)iA;
                    if (!int.TryParse(reader.ReadLine(), out iS))
                        return;
                    S = (Keys)iS;
                    if (!int.TryParse(reader.ReadLine(), out iD))
                        return;
                    D = (Keys)iD;
                }
                catch (Exception) { }
            }
        }

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

        public bool InvertX
        {
            set;
            get;
        }

        public bool InvertY
        {
            set;
            get;
        }

        public KeyboardConfig Config
        {
            get;
            set;
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
        }

        public void Open()
        {
            this.port.Open();
            this.MouseSensitivity = 1.0f;
            pumpThread = new Thread(new ThreadStart(this.pumpData));
            pumpThread.Start();
            this.run = true;
        }

        private void pumpData()
        {
            while (run)
            {
                string[] parts = null;
                try
                {
                    parts = port.ReadLine().Split(',');
                }
                catch (Exception) { continue; }
                if (parts.Length == 3)
                {
                    float xvel = 0.0f, yvel = 0.0f;
                    if (!float.TryParse(parts[0], out xvel))
                        return;
                    if (!float.TryParse(parts[1], out yvel))
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

                    Console.WriteLine(parts[0] + "," + parts[1] + "," + parts[2]);
                    Mouse.MoveRelative((int)(xvel * MouseSensitivity) * (InvertX ? -1 : 1), (int)(yvel * MouseSensitivity) * (InvertY ? -1 : 1));

                    if (leftClick && !pleftClick)
                        Mouse.ButtonDown(Mouse.MouseKeys.Left);
                    if (!leftClick && pleftClick)
                        Mouse.ButtonUp(Mouse.MouseKeys.Left);

                    if (rightClick && !prightClick)
                        Mouse.ButtonDown(Mouse.MouseKeys.Right);
                    if (!rightClick && prightClick)
                        Mouse.ButtonUp(Mouse.MouseKeys.Right);

                    if (crouch && !pcrouch)
                        Keyboard.KeyDown(this.Config.Crouch);
                    if (!crouch && pcrouch)
                        Keyboard.KeyUp(this.Config.Crouch);

                    if (W && !pW)
                        Keyboard.KeyDown(this.Config.W);
                    if (!W && pW)
                        Keyboard.KeyUp(this.Config.W);

                    if (A && !pA)
                        Keyboard.KeyDown(this.Config.A);
                    if (!A && pA)
                        Keyboard.KeyUp(this.Config.A);

                    if (S && !pS)
                        Keyboard.KeyDown(this.Config.S);
                    if (!S && pS)
                        Keyboard.KeyUp(this.Config.S);

                    if (D && !pD)
                        Keyboard.KeyDown(this.Config.D);
                    if (!D && pD)
                        Keyboard.KeyUp(this.Config.D);

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
