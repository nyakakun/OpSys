using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl;
using System.Windows.Forms;

namespace minimizer
{
    public class AutomatRW
    {
        protected Automat automat;

        public AutomatRW(Automat automat)
        {
            this.automat = automat;
        }
        public void Read(TextReader from, int countInputSignals, int countStates = -1)
        {
            for (int i = 0; i < countInputSignals; i++)
            {
                string? line = ReadLine(from);
                if (line == null) throw new Exception("Неожиданный конец данных");
                List<int> states = ConvertLine(line);
                if (countStates != -1 && states.Count != countStates)
                    throw new Exception("Неверное количество состояний");

                for (int index = 0; index < countStates; index++)
                {
                    automat[index][i].NextState = states[index] - 1;
                }
            }

            for (int i = 0; i < countInputSignals; i++)
            {
                string? line = ReadLine(from);
                if (line == null) throw new Exception("Неожиданный конец данных");
                List<int> signals = ConvertLine(line);
                if (countStates != -1 && signals.Count != countStates)
                    throw new Exception("Неверное количество состояний");

                for (int index = 0; index < countStates; index++)
                {
                    automat[index][i].Signal = signals[index];
                }
            }
        }
        public void Write(TextWriter to)
        {
            for (int indexState = 0; indexState < automat[0].Count; indexState++)
            {
                StringBuilder line = new();
                //line.Append(string.Format("{0}\t", automat[indexState].StateNum));
                for (int indexJump = 0; indexJump < automat.Count; indexJump++)
                {
                    if (automat[indexJump][indexState].NextState < 0)
                        line.Append(string.Format("  -  {0}", automat.Count - indexJump == 1 ? "" : " "));
                    else
                        line.Append(
                            string.Format(
                                "{0,2}/{1,2}{2}",
                                automat[indexJump][indexState].NextState,
                                automat[indexJump][indexState].Signal == -1 ? "-" : automat[indexJump][indexState].Signal,
                                automat.Count - indexJump == 1 ? "" : " "
                                )
                            );
                }
                to.WriteLine(line.ToString());
            }
        }
        public void VisualizeGraph(string filename)
        {
            Microsoft.Msagl.Drawing.Graph graph = new("");

            for (int indexJump = 0; indexJump < automat.Count; indexJump++)
            {
                //line.Append(string.Format("{0}\t", automat[indexState].StateNum));
                for (int indexState = 0; indexState < automat[0].Count; indexState++)
                {
                    if (automat[indexJump][indexState].NextState < 0) continue;
                    graph.AddEdge(
                        indexJump.ToString(),
                        automat[indexJump][indexState].NextState.ToString() + (automat[indexJump][indexState].Signal == -1 ? "" : ("/" + automat[indexJump][indexState].Signal.ToString()))
                    );
                }
                graph.FindNode(indexJump.ToString()).Attr.Shape = Microsoft.Msagl.Drawing.Shape.Ellipse;
            }

            Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new(graph);
            renderer.CalculateLayout();
            int scale = 10;
            Bitmap bitmap = new((int)graph.Width * scale, (int)graph.Height * scale, PixelFormat.Format32bppPArgb);
            renderer.Render(bitmap);
            bitmap.Save(filename);
        }

        private string? ReadLine(TextReader from)
        {
            string? line = "";
            while (line != null && line.Trim() == "") line = from.ReadLine();
            return line;
        }
        private List<int> ConvertLine(string line)
        {
            List<int> result = new List<int>();
            foreach (string intInString in line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (!int.TryParse(intInString, out int num))
                    if (intInString == "-") num = -1; else throw new Exception("");
                result.Add(num);
            }
            return result;
        }
        public Automat Automat { get { return automat; } set { automat = value; } }
    }
}