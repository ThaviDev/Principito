using System;
using System.Collections.Generic;
using System.Text; // Necesario para StringBuilder! :O
using UnityEngine;
using Doublsb.Dialog;

public enum Idioma { 
    Espaniol, 
    Ingles 
}

public class DialogLoader : MonoBehaviour
{
    public static DialogLoader Instance { get; private set; }
    public event Action<int> OnDialogueCompleted;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        CargarArchivo();
        OnDialogueRequested += HandleDialogRequest;
    }
    private class LineaDialogo
    {
        public int Id;
        public int Siguiente;
        public string Personaje;
        public string ColorAuto;
        public string Emocion;
        public string Espaniol;
        public string Ingles;
    }

    [Header("Referencias")]
    [SerializeField] private DialogManager dialogManager;

    [Header("Datos")]
    [Tooltip("Nombre del archivo de diálogos sin extensión (dentro de Resources/Data)")]
    [SerializeField] private string archivoDialogos = "DialogosLP";

    [Header("Idioma")]
    [SerializeField] private Idioma idioma = Idioma.Espaniol;
    [Header("Configuracion del texto")]
    [Tooltip("Tamaño por defecto del texto")]
    [SerializeField] private string m_DefaultTextSize = "60";

    public bool IsDialogueActive { get; private set; }

    // Se dispara cuando alguien pide un diálogo. El int es el Id inicial en el TSV.
    public event Action<int> OnDialogueRequested;

    private Dictionary<int, LineaDialogo> dialogos = new Dictionary<int, LineaDialogo>();
    private void OnDestroy()
    {
        OnDialogueRequested -= HandleDialogRequest;
    }
    public void PlayDialogue(int idInicial)
    {
        OnDialogueRequested?.Invoke(idInicial);
    }
    private void CargarArchivo()
    {
        TextAsset archivo = Resources.Load<TextAsset>("Data/" + archivoDialogos);
        if (archivo == null)
        {
            Debug.LogError($"No se pudo cargar el archivo de diálogos: Data/{archivoDialogos}");
            return;
        }
        dialogos = ParsearTSV(archivo.text);
        Debug.Log($"Diálogos cargados: {dialogos.Count} entradas.");
    }

    private void HandleDialogRequest(int idInicial)
    {
        if (dialogos == null || dialogos.Count == 0)
        {
            Debug.LogError("No hay diálogos cargados.");
            return;
        }

        if (IsDialogueActive) return;

        var lista = ConstruirDialogo(idInicial);

        if (lista.Count > 0)
        {
            // 1. Obtenemos el ÚLTIMO diálogo de la lista
            var ultimoDialogo = lista[lista.Count - 1];

            // 2. Guardamos su callback original (por si acaso tenía uno)
            var callbackOriginal = ultimoDialogo.Callback;

            // 3. Sobrescribimos el callback
            ultimoDialogo.Callback = () =>
            {
                // Ejecutamos el callback original si existía
                callbackOriginal?.Invoke();

                // Apagamos el estado de diálogo
                IsDialogueActive = false;
                // Debug.Log("Diálogo terminado, el jugador puede volver a moverse.");
                OnDialogueCompleted?.Invoke(idInicial);
            };

            // 4. Encendemos el estado y mostramos
            IsDialogueActive = true;
            dialogManager.Show(lista);
        }
        else
            Debug.LogWarning($"No se generó ningún diálogo para el Id {idInicial}.");
    }
    private Dictionary<int, LineaDialogo> ParsearTSV(string contenido)
    {
        var resultado = new Dictionary<int, LineaDialogo>();

        contenido = contenido.TrimStart('\uFEFF');
        contenido = contenido.Replace('\u00A0', ' ');
        contenido = contenido.Replace("\r\n", "\n").Replace('\r', '\n');

        string[] lineas = contenido.Split('\n');

        for (int i = 1; i < lineas.Length; i++)
        {
            string linea = lineas[i].Trim();
            if (string.IsNullOrEmpty(linea)) continue;

            string[] celdas = linea.Split('\t');

            if (celdas.Length < 6)
            {
                Debug.LogWarning($"Línea {i + 1} ignorada. Solo tiene {celdas.Length} columnas.");
                continue;
            }

            if (!int.TryParse(celdas[0].Trim(), out int id))
            {
                Debug.LogError($"Id inválido en línea {i + 1}: '{celdas[0]}'");
                continue;
            }

            int siguiente = -1;
            if (!string.IsNullOrWhiteSpace(celdas[1]))
                int.TryParse(celdas[1].Trim(), out siguiente);

            resultado[id] = new LineaDialogo
            {
                Id = id,
                Siguiente = siguiente,
                Personaje = celdas[2].Trim(),
                ColorAuto = celdas[3].Trim(),
                Emocion = celdas[4].Trim(),
                Espaniol = celdas[5].Trim(),
                Ingles = celdas.Length > 6 ? celdas[6].Trim() : ""
            };
        }

        return resultado;
    }

    private List<DialogData> ConstruirDialogo(int idInicial)
    {
        
        var idActual = idInicial;
        var lista = new List<DialogData>();
        int seguridad = 0;

        while (idActual != -1 && dialogos.ContainsKey(idActual) && seguridad < 1000)
        {
            var linea = dialogos[idActual];
            string textoBase = idioma == Idioma.Espaniol ? linea.Espaniol : linea.Ingles;
            // Usamos StringBuilder por rendimiento (buena práctica al concatenar strings)
            StringBuilder sb = new StringBuilder();
            // Inyectamos el tamaño por defecto (si no está vacío)
            if (!string.IsNullOrEmpty(m_DefaultTextSize))
            {
                sb.Append($"/size:{m_DefaultTextSize}/");
            }
            // Inyectamos el color de la columna Color Auto
            if (!string.IsNullOrEmpty(linea.ColorAuto))
            {
                sb.Append(linea.ColorAuto);
            }
            // Inyectamos la emoción
            if (!string.IsNullOrEmpty(linea.Emocion))
            {
                sb.Append(linea.Emocion);
            }
            // Finalmente, añadimos el texto del diálogo
            sb.Append(textoBase);
            string personaje = linea.Personaje;
            if (personaje == "No Char" || personaje == "Default")
            {
                personaje = "No Char";
            }
            lista.Add(new DialogData(sb.ToString(), personaje));
            idActual = linea.Siguiente;
            seguridad++;
        }
        return lista;
        /*
        while (idActual != -1 && dialogos.ContainsKey(idActual) && seguridad < 1000)
        {
            var linea = dialogos[idActual];
            string texto = idioma == Idioma.Espaniol ? linea.Espaniol : linea.Ingles;
            string personaje = linea.Personaje == "Default" ? "" : linea.Personaje;

            lista.Add(new DialogData(texto, personaje));
            idActual = linea.Siguiente;
            seguridad++;
        }

        return lista;
        */
    }
}