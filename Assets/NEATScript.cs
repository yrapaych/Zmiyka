using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /*class Program
    {
        static void Main(string[] args)
        {
            NEAT neat = new NEAT(200);
            neat.train(302);
            Genome champ = neat.champ();
        }
    }
    */
    class Conn
    {
        public int inpN = -1;
        public int outN = -1;
        public double w;
        public bool enbl;
        public int innov = -1;
    }

    class Node
    {
        public int n;
        public int type;////////////////// 0 = input, 1 = hidden, 2 = output
        public double inp = 0;

        /*
        public int getType() { return type; }
        public void setInp(double inp) { this.inp = inp; }
        public double getInp() { return inp; }
        public int getN() { return n; }*/

        public Node() { }
        public Node(int n, int type)
        {
            this.n = n;
            this.type = type;
        }
    }

    class Genome
    {
        public static Random rand = new Random();
        public List<Conn> conns = new List<Conn>();
        public List<Node> nodes = new List<Node>();
        public List<Node> inputs = new List<Node>();
        public List<Node> outputs = new List<Node>();
        public double fitness = 0;
        public int specname = -1;
        public Genome() { }
        public Genome(int inp, int outp)
        {
            for (int i = 0; i < inp; i++)
            {
                Node node = new Node(i, 0);
                nodes.Add(node);
                inputs.Add(node);
            }
            for (int i = inp; i < inp + outp; i++)
            {
                Node node = new Node(i, 2);
                nodes.Add(node);
                outputs.Add(node);
            }
        }
        public void Input(double[] inp)
        {
            if (inp.Length == inputs.Count)
            {
                for (int i = 0; i < inp.Length; i++)
                {
                    inputs[i].inp = inp[i];
                }
            }
        }
        public static double Sig(double x)
        {
            return (1 / (1 + Math.Exp(-x)));
        }
        public double[] output()
        {
            for (int i = 0; i < conns.Count; i++)
            {
                Conn con = conns[i];
                if (con.enbl)
                {
                    nodes[con.outN].inp = nodes[con.inpN].inp * con.w + nodes[con.outN].inp;
                    nodes[con.inpN].inp = 0;
                }
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].inp = Sig(nodes[i].inp);
            }
            double[] outp = new double[outputs.Count];
            for (int i = 0; i < outputs.Count; i++)
            {
                outp[i] = outputs[i].inp;
                outputs[i].inp = 0;
            }
            return outp;
        }

        Conn newCon(int inp, int outp)
        {
            if (AllGenes.allgenes.Count == 0)
            {
                Conn con1 = new Conn();
                con1.enbl = true;
                con1.inpN = inp;
                con1.outN = outp;
                con1.w = rand.NextDouble() - 0.5;
                con1.innov = 0;
                AllGenes.allgenes.Add(con1);
                return con1;
            }
            Conn con = new Conn();
            con.enbl = true;
            for (int i = 0; i < AllGenes.allgenes.Count; i++)
            {
                if ((inp == AllGenes.allgenes[i].inpN) && (outp == AllGenes.allgenes[i].outN))
                {
                    con.innov = AllGenes.allgenes[i].innov;
                    break;
                }
            }
            if (con.innov == -1)
            {
                con.innov = AllGenes.allgenes.Count;
                AllGenes.allgenes.Add(con);
            }
            con.inpN = inp;
            con.outN = outp;
            con.w = rand.NextDouble() - 0.5;
            return con;
        }

        public void mutate(double rc = 0.8, double rn = 0.8, double re = 0.95, double rw = 0.2)
        {
            if (rand.NextDouble() > rc)
            {
                Conn con = newCon(rand.Next(0, nodes.Count - 1), rand.Next(0, nodes.Count - 1));
                conns.Add(con);
            }
            if (rand.NextDouble() > rn)
            {
                if (conns.Count > 0)
                {
                    Conn buf = conns[rand.Next(0, conns.Count - 1)];
                    Node node = new Node(nodes.Count, 1);
                    Conn incon = newCon(buf.inpN, nodes.Count);
                    Conn outcon = newCon(nodes.Count, buf.outN);
                    conns.Add(incon);
                    conns.Add(outcon);
                    nodes.Add(node);
                    buf.enbl = false;
                }
            }
            if (rand.NextDouble() > re)
            {
                if (conns.Count > 0)
                {
                    Conn buf = conns[rand.Next(0, conns.Count - 1)];
                    buf.enbl = !buf.enbl;
                }
            }
            if (rand.NextDouble() > rw)
            {
                if (conns.Count > 0)
                {
                    conns[rand.Next(0, conns.Count - 1)].w += (rand.NextDouble() - 0.5);
                }
            }
        }
        public Genome clone()
        {
            Genome genome = new Genome();
            foreach (Conn con in conns)
            {
                Conn buf = new Conn();
                buf.enbl = con.enbl;
                buf.innov = con.innov;
                buf.inpN = con.inpN;
                buf.outN = con.outN;
                genome.conns.Add(buf);
            }
            foreach (Node nod in nodes)
            {
                Node buf = new Node(nod.n, nod.type);
                genome.nodes.Add(buf);
                if (buf.type == 0)
                {
                    genome.inputs.Add(buf);
                }
                else if (buf.type == 2)
                {
                    genome.outputs.Add(buf);
                }
            }
            genome.fitness = fitness;
            genome.specname = specname;
            return genome;
        }
    }

    static class AllGenes
    {
        public static List<Conn> allgenes = new List<Conn>();//
    }

    class Species
    {
        public List<Genome> population = new List<Genome>();
        public int lifetime = 0;
        public double popFit = 0;
        public int name;
        static int NAME = 0;
        Genome Champ;


        public Species()
        {
            this.name = NAME++;
        }

        public List<Species> checkPop()
        {
            List<Species> newspe = new List<Species>();
            for (int i = 0; i < population.Count; i++)
            {
                double x = 0;
                for (int j = 0; j < population.Count; j++)
                {
                    x += NEAT.compareGen(population[i], population[j]);
                }
                if (x / population.Count < 0.7)
                {
                    newspe.Add(new Species());
                    newspe[newspe.Count - 1].population.Add(population[i]);
                    population.RemoveAt(i);
                }
            }
            return newspe;
        }

        public int Count()
        {
            return population.Count;
        }

        public Genome champ()
        {
            Genome genome = population[0];
            foreach (Genome buf in population)
            {
                if (buf.fitness > genome.fitness)
                {
                    genome = buf;
                }
            }
            return genome;
        }
        public void simualate()
        {
            GameSim sim = new GameSim();//////////////////////here enter the sandbox you want

            popFit = 0;
            for (int i = 0; i < population.Count; i++)
            {
                population[i].fitness = sim.simulate(population[i]);////////////////////
            }
            for (int i = 0; i < population.Count; i++)
            {
                popFit += population[i].fitness;
            }
            popFit /= population.Count;
            lifetime++;
        }
        public void mutatePop()
        {
            for (int i = 0; i < population.Count; i++)
            {
                population[i].mutate();
            }
        }
    }

    class NEAT
    {
        List<Species> populations = new List<Species>();
        Genome[] allpops;
        int Pop;


        public NEAT(int Pop = 100)
        {
            this.Pop = Pop;
            allpops = new Genome[this.Pop];
            for (int i = 0; i < this.Pop; i++)
            {
                allpops[i] = new Genome(14, 4);
            }
            speciation();
        }

        void speciation()/////////////
        {
            for (int i = 0; i < Pop; i++)
            {
                for (int j = 0; j < Pop; j++)
                {
                    if (compareGen(allpops[i], allpops[j]) > 0.9)
                    {
                        if (allpops[i].specname == -1 && allpops[j].specname == -1)
                        {
                            Species species = new Species();
                            populations.Add(species);
                            species.population.Add(allpops[i]);
                            allpops[i].specname = species.name;
                            species.population.Add(allpops[j]);
                            allpops[j].specname = species.name;
                            species.popFit = (allpops[i].fitness + allpops[j].fitness) / 2;
                        }
                        else if (allpops[i].specname != -1 && allpops[j].specname != -1)
                        {
                            continue;
                        }
                        else if (allpops[i].specname != -1)
                        {
                            Species spe = null;
                            for (int k = 0; k < populations.Count; k++)
                            {
                                if (populations[k].name == allpops[i].specname)
                                {
                                    spe = populations[k];
                                    break;
                                }
                            }
                            spe.population.Add(allpops[j]);
                            allpops[j].specname = spe.name;
                        }
                        else if (allpops[j].specname != -1)
                        {
                            Species spe = null;
                            for (int k = 0; k < populations.Count; k++)
                            {
                                if (populations[k].name == allpops[j].specname)
                                {
                                    spe = populations[k];
                                    break;
                                }
                            }
                            spe.population.Add(allpops[i]);
                            allpops[i].specname = spe.name;
                        }
                    }
                }
            }


            for (int spe = 0; spe < populations.Count; spe++)
            {
                populations.AddRange(populations[spe].checkPop());
            }
        }

        public Genome champ()
        {
            double best = 0;
            Genome champ = null;

            for (int i = 0; i < populations.Count; i++)
            {
                Genome buf = populations[i].champ();
                if (buf.fitness > best)
                {
                    champ = buf;
                    best = buf.fitness;
                }
            }

            return champ;
        }

        public void train(int times)
        {
            for (int p = 1; p < times; p++)
            {
                for (int i = 0; i < populations.Count; i++)
                {
                    populations[i].simualate();
                    mutatePop();

                    speciation();
                    if (p % 5 == 0)
                    {
                        List<double> fits = new List<double>();
                        for (int pop1 = 0; pop1 < populations.Count; pop1++)
                        {
                            if (populations[pop1].lifetime < 10)
                            {
                                fits.Add(populations[pop1].popFit);
                            }
                            else
                            {
                                fits.Add(populations[pop1].popFit);
                            }
                        }
                        for (int pop = 0; fits.Count > Pop; pop++)
                        {
                            if (fits[pop] == fits.Min())
                            {
                                fits.RemoveAt(pop);
                                for (int gen = 0; 0 < populations[pop].population.Count; gen++)
                                {
                                    for (int i1 = 0; i1 < Pop; i1++)
                                    {
                                        if (allpops[i1].specname == pop)
                                        {
                                            allpops[i1] = null;
                                            if (Genome.rand.NextDouble() > 0.8)
                                            {
                                                allpops[i1] = new Genome(8, 4);
                                            }
                                            else
                                            {
                                                allpops[i1] = populations[Genome.rand.Next(0, populations.Count)].champ();
                                            }
                                        }
                                    }
                                }
                                populations.RemoveAt(pop);
                            }
                        }
                    }
                }
            }
        }

        public void mutatePop()
        {
            for (int i = 0; i < populations.Count; i++)
            {
                populations[i].mutatePop();
            }
        }

        /*public void mate()
        {
            for(int i = 0; i < populations.Count; i++)
            {
                for(int j = 0; j < populations[i].population.Count; j++)
                {
                    Genome[] buf = populations[i].champ();
                    Genome ch1 = mateHalf(buf[0], buf[1]);
                    Genome ch2 = mateHalf(buf[0], buf[2]);
                    Genome ch3 = mateHalf(buf[1], buf[2]);
                    List<Genome> bufGen = new List<Genome>();
                    for(int y = 0; bufGen.Count < populations[i].population.Count; y++)
                    {
                        bufGen.Add(buf[y % 3].clone());
                    }
                }
            }
        }*/

        public static double compareGen(Genome gen1, Genome gen2)
        {
            if (gen1.conns.Count == 0 && gen2.conns.Count == 0)
            {
                return 1;
            }
            else if (gen1.conns.Count == 0 || gen2.conns.Count == 0)
            {
                return 4 / (gen1.conns.Count + gen2.conns.Count);
            }

            int count = 0;
            for (int i = 0; i < gen1.conns.Count; i++)
            {
                foreach (Conn buf in gen2.conns)
                {
                    if (buf.innov == gen1.conns[i].innov)
                    {
                        count++;
                    }
                }
            }
            return (count / gen1.conns.Count + count / gen2.conns.Count) / 2;////////// coeficient of comparability
        }

        public static Genome mateFull(Genome gen1, Genome gen2)
        {
            Genome genome = new Genome(gen1.inputs.Count, gen1.outputs.Count);
            genome.fitness = (gen1.fitness + gen2.fitness) / 2;

            genome.conns.AddRange(gen1.conns);

            for (int i = 0; i < gen2.conns.Count; i++)
            {
                for (int j = 0; j < genome.conns.Count; j++)
                {
                    if (genome.conns[j].innov != gen2.conns[i].innov)
                    {
                        genome.conns.Add(gen2.conns[i]);
                    }
                }
            }

            bool containinp;
            bool containoutp;
            foreach (Conn con in genome.conns)
            {
                containinp = true;
                containoutp = true;
                foreach (Node nod in genome.nodes)
                {
                    if (nod.n == con.outN)
                    {
                        containoutp = false;
                    }
                    if (nod.n == con.inpN)
                    {
                        containinp = false;
                    }
                    if (!containinp && !containoutp) { break; }
                }
                if (containoutp) genome.nodes.Add(new Node(con.outN, 1));
                if (containinp) genome.nodes.Add(new Node(con.inpN, 1));
            }
            return genome;
        }

        public static Genome mateHalf(Genome gen1, Genome gen2)
        {
            Genome genome = new Genome(gen1.inputs.Count, gen1.outputs.Count);
            genome.fitness = (gen1.fitness + gen2.fitness) / 2;

            List<Conn> bufcon1 = new List<Conn>();
            bufcon1.AddRange(gen1.conns);
            List<Conn> bufcon2 = new List<Conn>();
            bufcon2.AddRange(gen2.conns);

            for (int i = 0; i < bufcon1.Count; i++)
            {
                bufcon1.RemoveAt(Genome.rand.Next(0, bufcon1.Count));
            }
            for (int i = 0; i < bufcon2.Count; i++)
            {
                bufcon2.RemoveAt(Genome.rand.Next(0, bufcon2.Count));
            }
            bufcon1.AddRange(bufcon2);

            bool containinp;
            bool containoutp;
            foreach (Conn con in bufcon1)
            {
                containinp = true;
                containoutp = true;
                foreach (Node nod in genome.nodes)
                {
                    if (nod.n == con.outN)
                    {
                        containoutp = false;
                    }
                    if (nod.n == con.inpN)
                    {
                        containinp = false;
                    }
                    if (!containinp && !containoutp) { break; }
                }
                if (containoutp) genome.nodes.Add(new Node(con.outN, 1));
                if (containinp) genome.nodes.Add(new Node(con.inpN, 1));
            }
            return genome;
        }
    }


    class GameSim
    {
        class Zmiyka
        {
            public body first;
            public body second;
            public body last;
            public bool alive = true;
            public class body
            {
                public bool head = false;
                public int x;
                public int y;
                public body next;
                public body(int x, int y, body next)
                {
                    this.x = x;
                    this.y = y;
                    this.next = next;
                }

            }
            public Zmiyka()
            {
                first = new body(0, 0, null);
                first.head = true;
                second = new body(0, -1, first);
                last = new body(0, -2, second);
            }


            public void grow(int x, int y)
            {
                body bod = new body(last.x, last.y, last);
                move(x, y);
                last = bod;
            }

            public void move(int x, int y)
            {
                body buf = last;
                while (!buf.head)
                {
                    buf.x = buf.next.x;
                    buf.y = buf.next.y;
                    buf = buf.next;
                }
                buf.x = x + buf.x;
                buf.y = y + buf.y;
            }
        }

        class Food
        {
            public int x = 5;
            public int y = 5;

            public void respawn()
            {
                x = Genome.rand.Next(-16, 16);
                y = Genome.rand.Next(-16, 16);
            }
        }


        double up = 16.5;
        double down = 0.5;
        double left = 16.5;
        double right = 16.5;
        double upright = 23.33;
        double upleft = 23.33;
        double downright = 23.33;
        double downleft = 23.33;
        double foodup = 0;
        double foodright = 0;
        double foodupright = 0;
        double foodupleft = 0;
        Zmiyka zmiyka = new Zmiyka();
        Food food = new Food();
        int x = 0;
        int y = 1;

        public GameSim()
        {

        }

        public void changMove(Double[] outp)
        {
            int symb = 0;
            double buf = 0;
            for (int i = 0; i < outp.Length; i++)
            {
                if (buf < outp[i])
                {
                    buf = outp[i];
                    symb = i;
                }
            }

            int x = this.x;
            int y = this.y;
            if (symb == 0)
            {
                x = -1;
                y = 0;
            }
            else if (symb == 1)
            {
                x = 1;
                y = 0;
            }
            else if (symb == 2)
            {
                x = 0;
                y = 1;
            }
            else if (symb == 3)
            {
                x = 0;
                y = -1;
            }
            if (x != zmiyka.second.x - zmiyka.first.x && y != zmiyka.second.y - zmiyka.first.y)
            {
                this.x = x;
                this.y = y;
            }
        }

        void upBody()
        {
            int y1 = zmiyka.first.y;
            int x1 = zmiyka.first.x;
            Zmiyka.body buf = zmiyka.last;
            bool upch = false;
            bool downch = false;
            up = 16.5 - y1;
            down = 16.5 + y1;
            for (int i = -16; i < 17; i++)
            {
                while (buf.next != null)
                {
                    if (buf.x == x1 && buf.y == i)
                    {
                        if (i > y1)
                        {
                            if (up > Math.Abs(i - y1) - 0.5)
                            {
                                up = Math.Abs(i - y1) - 0.5;
                                upch = true;
                            }
                        }
                        else
                        {
                            if (down > Math.Abs(i - y1) - 0.5)
                            {
                                down = Math.Abs(i - y1) - 0.5;
                                downch = true;
                            }
                        }
                    }
                    buf = buf.next;
                }
                buf = zmiyka.last;
            }
            if (!upch) up -= y;
            if (!downch) down += y;
        }

        double lengthXY(double x1, double y1, double x2, double y2)
        {
            return (Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
        }

        void rightUp()
        {
            int y1 = zmiyka.first.y;
            int x1 = zmiyka.first.x;
            int yb = zmiyka.first.y;
            int xb = zmiyka.first.x;
            while (xb > -16 && yb > -16)
            {
                xb--;
                yb--;
            }
            Zmiyka.body buf = zmiyka.last;
            bool upch = false;
            bool downch = false;
            while (xb < 16.5 && yb < 16.5)
            {
                while (buf.next != null)
                {
                    if (buf.x == xb && buf.y == yb)
                    {
                        if (yb > y1)
                        {
                            if (upright > lengthXY(x1, y1, buf.x - 0.5, buf.y - 0.5))
                            {
                                upright = lengthXY(x1, y1, buf.x - 0.5, buf.y - 0.5);
                                upch = true;
                            }
                        }
                        else
                        {
                            if (downleft > lengthXY(x1, y1, buf.x - 0.5, buf.y - 0.5))
                            {
                                downleft = lengthXY(x1, y1, buf.x - 0.5, buf.y - 0.5);
                                downch = true;
                            }
                        }
                    }
                    buf = buf.next;
                }
                buf = zmiyka.last;
                xb++;
                yb++;
            }
            if (!upch)
            {
                while (x1 < 16 && y1 < 16)
                {
                    x1++;
                    y1++;
                }
                upright = lengthXY(zmiyka.first.x, zmiyka.first.y, x1 - 0.5, y1 - 0.5);
            }
            if (!downch)
            {
                y1 = zmiyka.first.y;
                x1 = zmiyka.first.x;
                while (x1 > -16 && y1 > -16)
                {
                    x1--;
                    y1--;
                }
                downleft = lengthXY(zmiyka.first.x, zmiyka.first.y, x1 - 0.5, y1 - 0.5);
            }
        }
        void leftUp()
        {
            int y1 = zmiyka.first.y;
            int x1 = zmiyka.first.x;
            int yb = zmiyka.first.y;
            int xb = zmiyka.first.x;
            while (xb > -16 && yb > -16)
            {
                xb--;
                yb--;
            }
            Zmiyka.body buf = zmiyka.last;
            bool upch = false;
            bool downch = false;
            while (xb < 16.5 && yb < 16.5)
            {
                while (buf.next != null)
                {
                    if (buf.x == -xb && buf.y == yb)
                    {
                        if (yb > y1)
                        {
                            if (upleft > lengthXY(x1, y1, buf.x - 0.5, buf.y - 0.5))
                            {
                                upleft = lengthXY(x1, y1, buf.x - 0.5, buf.y - 0.5);
                                upch = true;
                            }
                        }
                        else
                        {
                            if (downright > lengthXY(x1, y1, buf.x - 0.5, buf.y - 0.5))
                            {
                                downright = lengthXY(x1, y1, buf.x - 0.5, buf.y - 0.5);
                                downch = true;
                            }
                        }
                    }
                    buf = buf.next;
                }
                yb++;
                xb++;
                buf = zmiyka.last;
            }
            if (!upch)
            {
                while (x1 < 16 && y1 < 16)
                {
                    x1++;
                    y1++;
                }
                upleft = lengthXY(zmiyka.first.x, zmiyka.first.y, x1 - 0.5, y1 - 0.5);
            }
            if (!downch)
            {
                y1 = zmiyka.first.y;
                x1 = zmiyka.first.x;
                while (x1 > -16 && y1 > -16)
                {
                    x1--;
                    y1--;
                }
                downright = lengthXY(zmiyka.first.x, zmiyka.first.y, x1 - 0.5, y1 - 0.5);
            }
        }

        void leftBody()
        {
            int y1 = zmiyka.first.y;
            int x1 = zmiyka.first.x;
            Zmiyka.body buf = zmiyka.last;
            bool upch = false;
            bool downch = false;
            right = 16.5 - x1;
            left = 16.5 + x1;
            for (int i = -16; i < 17; i++)
            {
                while (buf.next != null)
                {
                    if (buf.x == i && buf.y == y1)
                    {
                        if (i > x1)
                        {
                            if (right > Math.Abs(i - x1) - 0.5)
                            {
                                right = Math.Abs(i - x1) - 0.5;
                                upch = true;
                            }
                        }
                        else
                        {
                            if (left > Math.Abs(i - x1) - 0.5)
                            {
                                left = Math.Abs(i - x1) - 0.5;
                                downch = true;
                            }
                        }
                    }
                    buf = buf.next;
                }
            }
            if (!upch) right -= x;
            if (!downch) left += x;
        }
        void foodPos()
        {
            int x1 = zmiyka.first.x;
            int y1 = zmiyka.first.y;
            if (x1 == food.x)
            {
                foodup = food.y - y1;
            }
            else
            {
                foodup = 0;
            }
            if (y1 == food.y)
            {
                foodright = food.x - x1;
            }
            else
            {
                foodright = 0;
            }
            x1 -= 16;
            y1 -= 16;
            while (x1 < 16 && y1 < 16)
            {
                if (food.x == x1 && food.y == y1)
                {
                    if (food.y > zmiyka.first.y)
                    {
                        foodupright = lengthXY(zmiyka.first.x, zmiyka.first.y, food.x, food.y);
                    }
                    else
                    {
                        foodupright = -lengthXY(zmiyka.first.x, zmiyka.first.y, food.x, food.y);
                    }
                    break;
                }
                else
                {
                    foodupright = 0;
                }
                if (food.x == -x1 && food.y == y1)
                {
                    if (food.y > zmiyka.first.y)
                    {
                        foodupleft = lengthXY(zmiyka.first.x, zmiyka.first.y, food.x, food.y);
                    }
                    else
                    {
                        foodupleft = -lengthXY(zmiyka.first.x, zmiyka.first.y, food.x, food.y);
                    }
                    break;
                }
                else
                {
                    foodupleft = 0;
                }
                x1++;
                y1++;
            }
        }

        public void grow()
        {
            upBody();
            leftBody();
            rightUp();
            leftUp();
            foodPos();
            zmiyka.grow(x, y);
        }

        void move()
        {
            upBody();
            leftBody();
            rightUp();
            leftUp();
            foodPos();
            zmiyka.move(x, y);
        }

        void gameOver()
        {
            zmiyka.alive = ((zmiyka.first.x > -16) && (zmiyka.first.x < 16) && (zmiyka.first.y > -16) && (zmiyka.first.y < 16) && (down > 0) && (up > 0) && (left > 0) && (right > 0));
        }

        double[] INP()
        {
            double[] a = new double[14];
            a[0] = down;
            a[1] = up;
            a[2] = left;
            a[3] = right;
            a[4] = upright;
            a[5] = downleft;
            a[6] = upleft;
            a[7] = downright;
            a[8] = foodup;
            a[9] = foodright;
            a[10] = foodupright;
            a[11] = foodupleft;
            a[12] = food.x - zmiyka.first.x;
            a[13] = food.y - zmiyka.first.y;
            return a;
        }
        void refresh()
        {
            up = 16.5;
            down = 0.5;
            left = 16.5;
            right = 16.5;
            upright = 23.33;
            upleft = 23.33;
            downright = 23.33;
            downleft = 23.33;
            foodup = 0;
            foodright = 0;
            foodupright = 0;
            foodupleft = 0;

            zmiyka = new Zmiyka();
            food = new Food();
            x = 0;
            y = 1;
        }

        public double simulate(Genome gen)
        {
            double moves = 0;
            double atefood = 0;
            while (zmiyka.alive)
            {
                moves++;
                gameOver();
                gen.Input(INP());
                changMove(gen.output());
                if (food.x == zmiyka.first.x && food.y == zmiyka.first.y)
                {
                    grow();
                    food.respawn();
                    atefood++;
                }
                else
                {
                    move();
                }
                if (moves > 500) zmiyka.alive = false;
            }
            zmiyka = new Zmiyka();
            x = 0;
            y = 1;


            refresh();
            return ((atefood * 1000) + moves) / (gen.nodes.Count);
        }
    }
}



