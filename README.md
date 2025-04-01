# Conferencia 1S2025 (Ejemplos Básicos)

| Nombre | Usuario Git |
| --- | --- |
| Juan Urbina | [Iskandar1412](https://github.com/Iskandar1412) |

## Código Python (Linux)

```sh
cd Python
python3 Blockchain.py
```

## Código C#

> Creación proyectos (Linux)

- Primero tener instaladas las dependencias (`dotnet-sdk`, `dotnet-runtime`)

```sh
dotnet new console -n <nombre_proyecto>
```

- Como vamos a usar para serializar y descerializar archivos JSON vamos a requerir `Newtonsoft.Json` 

```sh
# Dentro de la carpeta raíz del proyecto => cd <carpeta_proyecto>
dotnet add package Newtonsoft.Json
```

> Correr programa C#

```sh
# Dentro de la carpeta raíz del proyecto => cd <carpeta_proyecto>
dotnet run
```