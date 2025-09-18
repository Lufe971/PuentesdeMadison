using System;
using System.Collections.Generic;

namespace CosechaCaballo
{
    class Program
    {
        // Movimientos del caballo: clave -> (dx, dy)
        static readonly Dictionary<string, (int dx, int dy)> MOVIMIENTOS = new()
        {
            {"UL", (-1,  2)}, // arriba izquierda
            {"UR", ( 1,  2)}, // arriba derecha
            {"LU", (-2,  1)}, // izquierda arriba
            {"LD", (-2, -1)}, // izquierda abajo
            {"RU", ( 2,  1)}, // derecha arriba
            {"RD", ( 2, -1)}, // derecha abajo
            {"DL", (-1, -2)}, // abajo izquierda
            {"DR", ( 1, -2)}  // abajo derecha
        };

        static (int x, int y) PosToCoord(string pos)
        {
            if (string.IsNullOrWhiteSpace(pos) || pos.Length < 2)
                throw new FormatException("Posición inválida: " + pos);

            pos = pos.Trim().ToUpper();
            char col = pos[0];
            if (col < 'A' || col > 'H')
                throw new FormatException("Columna inválida: " + col);

            // fila puede ser '1'..'8' (en este problema siempre 1 dígito)
            if (!int.TryParse(pos.Substring(1), out int row) || row < 1 || row > 8)
                throw new FormatException("Fila inválida en: " + pos);

            return (col - 'A', row - 1); // 0-based
        }

        static string CoordToPos(int x, int y)
        {
            if (x < 0 || x > 7 || y < 0 || y > 7) return "Fuera";
            char col = (char)('A' + x);
            int row = y + 1;
            return $"{col}{row}";
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Ingrese ubicación de los frutos (ej: C4+,C7*,E3-,E1=,H4*):");
            string frutosLine = Console.ReadLine() ?? "";
            Console.WriteLine("Ingrese posición inicial del caballo (ej: B6):");
            string caballoLine = Console.ReadLine() ?? "";
            Console.WriteLine("Ingrese los movimientos del caballo (ej: DR,RD,LD,...):");
            string movimientosLine = Console.ReadLine() ?? "";

            // Parsear frutos
            var frutos = new Dictionary<(int, int), char>();
            var tokensFrutos = frutosLine.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var raw in tokensFrutos)
            {
                var s = raw.Trim();
                if (s.Length < 3) continue; // ignorar
                char simbolo = s[^1]; // último char
                string posStr = s.Substring(0, s.Length - 1);
                try
                {
                    var coord = PosToCoord(posStr);
                    frutos[coord] = simbolo; // si hay colisión, sobrescribe (útil en validaciones)
                }
                catch (FormatException fe)
                {
                    Console.WriteLine($"Aviso: fruto ignorado por formato inválido '{s}': {fe.Message}");
                }
            }

            // Posición inicial caballo
            (int x, int y) posCaballo;
            try
            {
                posCaballo = PosToCoord(caballoLine.Trim());
            }
            catch (FormatException fe)
            {
                Console.WriteLine("Posición inicial inválida. Terminando. " + fe.Message);
                return;
            }

            // Movimientos
            var movimientos = movimientosLine.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var recogidos = new List<char>();

            int cx = posCaballo.x, cy = posCaballo.y;
            Console.WriteLine($"Caballo inicia en {CoordToPos(cx, cy)}");

            foreach (var raw in movimientos)
            {
                var mv = raw.Trim().ToUpper();
                if (!MOVIMIENTOS.ContainsKey(mv))
                {
                    Console.WriteLine($"Movimiento inválido '{mv}' — se ignora.");
                    continue;
                }

                var d = MOVIMIENTOS[mv];
                int nx = cx + d.dx;
                int ny = cy + d.dy;

                if (nx < 0 || nx > 7 || ny < 0 || ny > 7)
                {
                    Console.WriteLine($"Movimiento {mv} llevaría fuera del tablero (desde {CoordToPos(cx, cy)} hacia {nx},{ny}) — se ignora.");
                    continue;
                }

                cx = nx; cy = ny;
                string posNow = CoordToPos(cx, cy);
                Console.WriteLine($"Movimiento {mv}: caballo en {posNow}");

                if (frutos.TryGetValue((cx, cy), out char simb))
                {
                    recogidos.Add(simb);
                    frutos.Remove((cx, cy));
                    Console.WriteLine($"  -> Recolectó '{simb}' en {posNow}");
                }
            }

            if (recogidos.Count == 0)
                Console.WriteLine("Los frutos recogidos son: ninguno");
            else
                Console.WriteLine("Los frutos recogidos son: " + string.Join(" ", recogidos));

            Console.WriteLine("\nFin. Presione ENTER para cerrar.");
            Console.ReadLine();
        }
    }
}
}