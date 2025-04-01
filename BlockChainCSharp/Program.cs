using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

public class Bloque {
    public int Index { get; set; }
    public string MarcaTiempo { get; set; }
    public object Cuerpo { get; set; }
    public string HashAnterior { get; set; }
    public string Hash { get; set; }

    public Bloque(int index, string marcaTiempo, object cuerpo, string hashAnterior) {
        Index = index;
        MarcaTiempo = marcaTiempo;
        Cuerpo = cuerpo;
        HashAnterior = hashAnterior;
        Hash = ObtenerHash();
    }

    public string ObtenerHash() {
        var bloqueAString = JsonConvert.SerializeObject(new
        {
            Index,
            MarcaTiempo,
            Cuerpo,
            HashAnterior
        });
        using (var sha256 = SHA256.Create()) {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(bloqueAString));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

    public Dictionary<string, object> ObtenerBloque() {
        return new Dictionary<string, object> {
            { "index", Index },
            { "marca_tiempo", MarcaTiempo },
            { "cuerpo", Cuerpo },
            { "hash_anterior", HashAnterior },
            { "hash", Hash }
        };
    }

    public static Bloque ObtenerBloquesArchivo(Dictionary<string, object> data) {
        return new Bloque(
            Convert.ToInt32(data["index"]),
            data["marca_tiempo"].ToString(),
            data["cuerpo"],
            data["hash_anterior"].ToString()
        );
    }
}

public class Blockchain {
    private string filename;
    public List<Bloque> Cadena { get; set; }

    public Blockchain(string filename = "blockchain.json") {
        this.filename = filename;
        Cadena = new List<Bloque>();
        CargarCadena();
    }

    public void CargarCadena() {
        if (File.Exists(filename)) {
            var datos = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(File.ReadAllText(filename));
            foreach (var data in datos) {
                Cadena.Add(Bloque.ObtenerBloquesArchivo(data));
            }
        }
        else {
            CrearBloqueGenesis();
        }
    }

    public void CrearBloqueGenesis() {
        var bloqueGenesis = new Bloque(0, DateTime.Now.ToString(), "Bloque Genesis", "0");
        Cadena.Add(bloqueGenesis);
        GuardarBloque();
    }

    public void AgregarBloque(object cuerpo) {
        var bloqueAnterior = Cadena[Cadena.Count - 1];
        var nuevoBloque = new Bloque(
            bloqueAnterior.Index + 1,
            DateTime.Now.ToString(),
            cuerpo,
            bloqueAnterior.Hash
        );
        Cadena.Add(nuevoBloque);
        GuardarBloque();
    }

    public void GuardarBloque() {
        var cuerpoCadena = new List<Dictionary<string, object>>();
        foreach (var block in Cadena) {
            cuerpoCadena.Add(block.ObtenerBloque());
        }
        File.WriteAllText(filename, JsonConvert.SerializeObject(cuerpoCadena, Formatting.Indented));
    }

    public void MostrarContenidoBlockchain() {
        foreach (var block in Cadena) {
            Console.WriteLine(JsonConvert.SerializeObject(block.ObtenerBloque(), Formatting.Indented));
        }
    }
}

class Program {
    static void Main() {
        // Creamos nuestro blockchain
        var nuevaCadena = new Blockchain();

        // Aquí vamos a añadir bloques (transacciones/documentos/etc)
        nuevaCadena.AgregarBloque(new { sender = "Alice", receiver = "Bob", amount = 50 });
        nuevaCadena.AgregarBloque(new { document = "Contrato_123", status = "firmado" });
        nuevaCadena.AgregarBloque(new { vote = "Candidato_A", voter_id = "user_42" });

        // Mostramos la cadena de bloques (blockchain), en este caso la guardamos en un JSON
        Console.WriteLine("--- Blockchain guardada en 'blockchain.json' ---");
        nuevaCadena.MostrarContenidoBlockchain();
    }
}
