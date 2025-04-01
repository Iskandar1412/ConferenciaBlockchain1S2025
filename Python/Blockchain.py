import json
import hashlib
import os
from datetime import datetime
from typing import Dict, List, Any

class Bloque:
    def __init__(self, index: int, marca_tiempo: str, cuerpo: Any, hash_anterior: str):
        self.index = index
        self.marca_tiempo = marca_tiempo
        self.cuerpo = cuerpo
        self.hash_anterior = hash_anterior
        self.hash = self.obtener_hash()

    def obtener_hash(self) -> str:
        bloque_a_string = json.dumps({
            "index": self.index,
            "marca_tiempo": self.marca_tiempo,
            "cuerpo": self.cuerpo,
            "hash_anterior": self.hash_anterior
        }, sort_keys=True).encode()
        return hashlib.sha256(bloque_a_string).hexdigest()

    def obtener_bloque(self) -> Dict[str, Any]:
        return {
            "index": self.index,
            "marca_tiempo": self.marca_tiempo,
            "cuerpo": self.cuerpo,
            "hash_anterior": self.hash_anterior,
            "hash": self.hash
        }

    @staticmethod
    def obtener_bloques_archivo(data: Dict[str, Any]) -> 'Bloque':
        return Bloque(
            index=data["index"],
            marca_tiempo=data["marca_tiempo"],
            cuerpo=data["cuerpo"],
            hash_anterior=data["hash_anterior"]
        )

class Blockchain:
    def __init__(self, filename: str = "blockchain.json"):
        self.filename = filename
        self.cadena: List[Bloque] = []
        self.cargar_cadena()

    def cargar_cadena(self):
        if os.path.exists(self.filename):
            with open(self.filename, "r") as f: # se va a verificar que exista o no un bloque en caso que no exista, simplemente se crea desde el primero
                datos = json.load(f)
                self.cadena = [Bloque.obtener_bloques_archivo(b) for b in datos]
        else:
            self.crear_bloque_genesis()

    def crear_bloque_genesis(self): # Creamos el bloque Genesis
        bloque_genesis = Bloque(0, str(datetime.now()), "Bloque Genesis", "0")
        self.cadena.append(bloque_genesis)
        self.guardar_bloque()

    def agregar_bloque(self, cuerpo: Any):
        bloque_anterior = self.cadena[-1]
        nuevo_bloque = Bloque(
            index=bloque_anterior.index + 1,
            marca_tiempo=str(datetime.now()),
            cuerpo=cuerpo,
            hash_anterior=bloque_anterior.hash
        )
        self.cadena.append(nuevo_bloque)
        self.guardar_bloque()

    def guardar_bloque(self):
        cuerpo_cadena = [block.obtener_bloque() for block in self.cadena]
        with open(self.filename, "w") as f:
            json.dump(cuerpo_cadena, f, indent=4)

    def mostrar_contenido_blockchain(self):
        for block in self.cadena:
            print(json.dumps(block.obtener_bloque(), indent=4))


# Simular el funcionamiento (transacciones)
if __name__ == "__main__":
    # Creamos nuestro blochchain
    nueva_cadena = Blockchain()

    # Aquí vamos a añadir bloques (transacciones/documentos/etc)
    nueva_cadena.agregar_bloque({"sender": "Alice", "receiver": "Bob", "amount": 50})
    nueva_cadena.agregar_bloque({"document": "Contrato_123", "status": "firmado"})
    nueva_cadena.agregar_bloque({"vote": "Candidato_A", "voter_id": "user_42"})

    # Mostramos la cadena de bloques (blockchain), en este caso la guardamos en un JSON
    print("--- Blockchain guardada en 'blockchain.json' ---")
    nueva_cadena.mostrar_contenido_blockchain()
