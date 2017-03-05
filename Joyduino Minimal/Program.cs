/* 
 * Author: Andrea Fioraldi <andreafioraldi@gmail.com>
 * License: MIT
 */
using System.Collections.Generic;
using WindowsInput;
using System.IO.Ports;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace Joyduino_Minimal
{
    class Program
    {
        const int MOUSE_STEP = 8;

        public static Dictionary<char, KeyList> table = new Dictionary<char, KeyList>()
        {
			/*
				Associate with each joypad button a list of keys.
				A key contains the two action to do when the button is pressed and released.
				Multiple keys can be associated with a single button.
				
				Actually the association is based on COD5 WaW PC game, adjust it for the game that you want use.
			*/
			//pad up
            {'a', new KeyList() { new Key(VirtualKeyCode.VK_F, InputSimulator.SimulateKeyDown, VirtualKeyCode.VK_F, InputSimulator.SimulateKeyUp) } },
            //pad down
			{'b', new KeyList() { new Key(VirtualKeyCode.CONTROL, InputSimulator.SimulateKeyDown, VirtualKeyCode.CONTROL, InputSimulator.SimulateKeyUp) } },
            //pad left
			{'c', new KeyList() { new Key(VirtualKeyCode.LEFT, InputSimulator.SimulateKeyDown, VirtualKeyCode.LEFT, InputSimulator.SimulateKeyUp) } },
            //pad right
			{'d', new KeyList() { new Key(VirtualKeyCode.VK_5, InputSimulator.SimulateKeyDown, VirtualKeyCode.VK_5, InputSimulator.SimulateKeyUp) } },
            //cross
			{'e', new KeyList() { new Key(VirtualKeyCode.SPACE, InputSimulator.SimulateKeyDown, VirtualKeyCode.SPACE, InputSimulator.SimulateKeyUp) } },
            //square
			{'f', new KeyList() { new Key(VirtualKeyCode.VK_R, InputSimulator.SimulateKeyDown, VirtualKeyCode.VK_R, InputSimulator.SimulateKeyUp) } },
            //circle
			{'g', new KeyList() {
                new Key(VirtualKeyCode.VK_C, InputSimulator.SimulateKeyDown, VirtualKeyCode.VK_C, InputSimulator.SimulateKeyDown),
                new Key(VirtualKeyCode.CONTROL, InputSimulator.SimulateKeyDown, VirtualKeyCode.CONTROL, InputSimulator.SimulateKeyDown),
                new Key(VirtualKeyCode.SPACE, InputSimulator.SimulateKeyDown, VirtualKeyCode.SPACE, InputSimulator.SimulateKeyDown)
            } },
            //triangle
			{'h', new KeyList() { new Key(VirtualKeyCode.VK_1, InputSimulator.SimulateKeyDown, VirtualKeyCode.VK_1, InputSimulator.SimulateKeyUp) } },
            //L1
			{'i', new KeyList() { new Key(VirtualKeyCode.VK_4, InputSimulator.SimulateKeyDown, VirtualKeyCode.VK_4, InputSimulator.SimulateKeyUp) } },
            //L2
			{'j', new KeyList() { new Key(VirtualKeyCode.RBUTTON, InputSimulator.SimulateKeyPress, VirtualKeyCode.RBUTTON, InputSimulator.SimulateKeyPress) } },
            //R1
			{'k', new KeyList() { new Key(VirtualKeyCode.VK_G, InputSimulator.SimulateKeyDown, VirtualKeyCode.VK_G, InputSimulator.SimulateKeyUp) } },
            //R2
			{'l', new KeyList() { new Key(VirtualKeyCode.LBUTTON, InputSimulator.SimulateKeyDown, VirtualKeyCode.LBUTTON, InputSimulator.SimulateKeyUp) } },
            //Select
			{'m', new KeyList() { new Key(VirtualKeyCode.VK_6, InputSimulator.SimulateKeyDown, VirtualKeyCode.VK_6, InputSimulator.SimulateKeyUp) } },
            //Start
			{'n', new KeyList() { new Key(VirtualKeyCode.ESCAPE, InputSimulator.SimulateKeyDown, VirtualKeyCode.ESCAPE, InputSimulator.SimulateKeyUp) } },
            //R3
			{'o', new KeyList() { new Key(VirtualKeyCode.VK_V, InputSimulator.SimulateKeyDown, VirtualKeyCode.VK_V, InputSimulator.SimulateKeyUp) } },
            //L3
			{'p', new KeyList() { new Key(VirtualKeyCode.LSHIFT, InputSimulator.SimulateKeyDown, VirtualKeyCode.LSHIFT, InputSimulator.SimulateKeyUp) } }
        };

		//move mouse proportionally to the analog coordinates
        public static void AnalogMouse(int x, int y)
        {
            if (x >= 98 && x <= 142)
                x = 0;
            else x = (x - 128) / MOUSE_STEP;
            if (y >= 85 && y <= 131)
                y = 0;
            else y = (y - 128) / MOUSE_STEP;
            Cursor.Position = new Point(Cursor.Position.X + x, Cursor.Position.Y + y);
        }

		//press WSAD keys proportionally to the analog coordinates change
        public class AnalogKeys
        {
            public VirtualKeyCode oldKey = 0;

            public static int Calibrate(int c)
            {
                if (c > 180 || c < 80)
                    return -128 + c;
                return 0;
            }

            //default WSAD pad keys
            public VirtualKeyCode Up = VirtualKeyCode.VK_W;
            public VirtualKeyCode Down = VirtualKeyCode.VK_S;
            public VirtualKeyCode Left = VirtualKeyCode.VK_A;
            public VirtualKeyCode Right = VirtualKeyCode.VK_D;

            public void Input(int x, int y)
            {
                int xs = Calibrate(x);
                int ys = Calibrate(y);

                VirtualKeyCode newKey = 0;
                if (Math.Abs(xs) >= Math.Abs(ys))
                {
                    if (xs > 0)
                        newKey = Right;
                    else if (xs < 0)
                        newKey = Left;
                    else newKey = 0;
                }
                else
                {
                    if (ys > 0)
                        newKey = Down;
                    else if (ys < 0)
                        newKey = Up;
                    else newKey = 0;
                }
                if(newKey != oldKey)
                {
                    if (oldKey != 0)
                        InputSimulator.SimulateKeyUp(oldKey);
                    if (newKey != 0)
                        InputSimulator.SimulateKeyDown(newKey);
                    oldKey = newKey;
                }
            }
        }

        public delegate void SimulateKeyAction(VirtualKeyCode keyCode);

		
        public class Key
        {
            public VirtualKeyCode code1;
            public VirtualKeyCode code2;
            public SimulateKeyAction down;
            public SimulateKeyAction up;

            public Key(VirtualKeyCode code1, SimulateKeyAction down, VirtualKeyCode code2, SimulateKeyAction up)
            {
                this.code1 = code1;
                this.down = down;
                this.code2 = code2;
                this.up = up;
            }
             
            public void Down()
            {
                down(code1);
            }

            public void Up()
            {
                up(code2);
            }
        }

        public class KeyList : List<Key>
        {
            public int index = 0;

            public void Reset()
            {
                index = 0;
            }

            public void Next()
            {
                if (index == Count - 1)
                    index = 0;
                else ++index;
            }

            public Key Get()
            {
                return this[index];
            }

            public Key GetNext()
            {
                Key k = this[index];
                Next();
                Console.WriteLine(index);
                return k;
            }
        }


        static int Main(string[] args)
        {

            string[] pnames = SerialPort.GetPortNames();
            string name = "";
            if(pnames.Length == 0)
            {
                Console.Error.WriteLine("Errore: Nessuna porta seriale connessa");
                return 1;
            }
            else if (pnames.Length == 1)
            {
                Console.WriteLine("Una porta seriale rivevata: " + pnames[0]);
                name = pnames[0];
            }
            else
            {
                Console.WriteLine("Rilevate più porte seriali:");
                foreach(string s in pnames)
                    Console.WriteLine(s);
                string r = "";
                while(true)
                {
                    Console.Write("Selezionane una >> ");
                    r = Console.ReadLine();
                    if (pnames.Contains(r))
                        break;
                    else Console.Error.WriteLine("Errore: Nome porta inesistente, riprova.");
                }
            }

            SerialPort serial = new SerialPort(name);
            serial.NewLine = "\x03";
            Console.WriteLine("Sto aprendo la porta seriale...");
            try {
                serial.Open();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Errore: " + e.Message);
                return 1;
            }
            Console.WriteLine("Porta aperta con successo.");
            string read;
            Console.WriteLine("Attendo feedback dal controller...");
            read = serial.ReadLine();
            if (read == "error")
            {
                Console.Error.WriteLine("Errore: Controller non rilevato");
                return 1;
            }
            Console.WriteLine("Controller pronto all'uso.");

			/*
			 left analog ---> WSAD keys
			 right analog --> mouse
			*/
            AnalogKeys lanalog = new AnalogKeys();

            while (true)
            {
                read = "";
                int r;
                while((r = serial.ReadByte()) != 3)
                    read += (char)r;
                
				foreach (char c in read)
                {
                    try { table[c].Get().Down(); } catch (Exception ex) { /*Console.WriteLine(ex.Message);*/ }
                }
                read = "";
                while ((r = serial.ReadByte()) != 3)
                    read += (char)r;
                
				foreach (char c in read)
                {
                    try { table[c].GetNext().Up(); } catch (Exception ex) { /*Console.WriteLine(ex.Message);*/ }
                }

                int buf0 = serial.ReadByte();
                int buf1 = serial.ReadByte();
                int buf2 = serial.ReadByte();
                int buf3 = serial.ReadByte();
                //L Analog
                lanalog.Input(buf0, buf1);
                //R Analog
                AnalogMouse(buf2, buf3);
            }
        }
    }
}
