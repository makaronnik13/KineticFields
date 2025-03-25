using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class KineticGraphManager
{
    // Словарь для кэширования связок KineticGraph - KineticSceneGraph
    private static Dictionary<KineticGraph, KineticSceneGraph> graphCache = new Dictionary<KineticGraph, KineticSceneGraph>();

    // Статический конструктор для подписки на события PlayModeStateChanged
    static KineticGraphManager()
    {
        // Подписка на изменение состояния PlayMode
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    
    // Метод, который вызывается при изменении состояния PlayMode
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // Если PlayMode начинается, очищаем кэш
        if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingPlayMode)
        {
            Debug.Log("Clear cache");
            // Очистка кэша при входе и выходе из PlayMode
            EditorApplication.delayCall += () => graphCache.Clear();
        }
    }

    
    // Метод получения KineticSceneGraph для заданного KineticGraph
    public static KineticSceneGraph GetKineticSceneGraph(KineticGraph kineticGraph)
    {
        // Если сцена уже в кэше, возвращаем из кэша
        if (graphCache.ContainsKey(kineticGraph))
        {
            return graphCache[kineticGraph];
        }

        // Ищем соответствующий KineticSceneGraph для KineticGraph
        KineticSceneGraph sceneGraph = null;

        // В редакторе
#if UNITY_EDITOR
        // В редакторе мы можем искать все объекты типа KineticSceneGraph
        var allSceneGraphs = GameObject.FindObjectsOfType<KineticSceneGraph>();
        
        foreach (var sg in allSceneGraphs)
        {
            if (sg.graph == kineticGraph)  // Если KineticSceneGraph связан с данным KineticGraph
            {
                sceneGraph = sg;
                break;
            }
        }
#else

        // В рантайме предполагаем, что сцена один раз создается или передается в виде объекта сцены
        if (sceneGraph == null)
        {
            // Проверяем, возможно сцена доступна в рантайме
            sceneGraph = Object.FindObjectOfType<KineticSceneGraph>(); // Ищем первый доступный объект
        }
#endif
        
        // Если найден KineticSceneGraph, добавляем в кэш
        if (sceneGraph != null)
        {
            graphCache[kineticGraph] = sceneGraph;
        }
        
        return sceneGraph;
    }
}