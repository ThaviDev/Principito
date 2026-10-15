using System;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public enum Idioma { 
    Español, 
    Inglés 
}

public class DialogLoader : MonoBehaviour
{
    public static DialogLoader Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        CargarArchivo();
        OnDialogueRequested += HandleDialogueRequest;
    }
    private class LineaDialogo
    {
        public int Id;
        public int Siguiente;
        public string Personaje;
        public string Espaniol;
        public string Ingles;
    }

    [Header("Referencias")]
    [SerializeField] private DialogManager dialogManager;

    [Header("Datos")]
    [Tooltip("Nombre del archivo de diálogos sin extensión (dentro de Resources/Data)")]
    [SerializeField] private string archivoDialogos = "DialogosLP";

    [Header("Idioma")]
    [SerializeField] private Idioma idioma = Idioma.Español;

    // Se dispara cuando alguien pide un diálogo. El int es el Id inicial en el TSV.
    public event Action<int> OnDialogueRequested;

    private Dictionary<int, LineaDialogo> dialogos = new Dictionary<int, LineaDialogo>();
    private void OnDestroy()
    {
        OnDialogueRequested -= HandleDialogueRequest;
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

    private void HandleDialogueRequest(int idInicial)
    {
        if (dialogos == null || dialogos.Count == 0)
        {
            Debug.LogError("No hay diálogos cargados.");
            return;
        }

        var lista = ConstruirDialogo(idInicial);
        if (lista.Count > 0)
            dialogManager.Show(lista);
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

            if (celdas.Length < 4) continue;

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
                Espaniol = celdas.Length > 3 ? celdas[3].Trim() : "",
                Ingles = celdas.Length > 4 ? celdas[4].Trim() : ""
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
            string texto = idioma == Idioma.Español ? linea.Espaniol : linea.Ingles;
            string personaje = linea.Personaje == "Default" ? "" : linea.Personaje;

            lista.Add(new DialogData(texto, personaje));
            idActual = linea.Siguiente;
            seguridad++;
        }

        return lista;
    }
}