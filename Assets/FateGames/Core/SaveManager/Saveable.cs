using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class Saveable : MonoBehaviour
{
    public UnityEngine.Object target;
    [SerializeField] List<FateFieldKey> fieldKeys = new();
    [SerializeField] bool isAwaken = false;
    FieldInfo[] fieldInfo = null;
    void Awake()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            if (!isAwaken)
            {
                isAwaken = true;
                for (int i = 0; i < fieldKeys.Count; i++)
                {
                    fieldKeys[i].key = "";
                }
                OnValidate();
            }
            
        }
        else
        {
            if (fieldKeys.Count > 0)
            {
                SaveManager.Instance.Register(this);
                LoadFields();
            }
        }

    }

    public bool IsFieldActive(string fieldName)
    {
        for (int i = 0; i < fieldKeys.Count; i++)
        {
            if (fieldKeys[i].fieldName == fieldName)
                return true;
        }
        return false;
    }

    public bool AddFieldKey(string fieldName, bool withoutKey = false)
    {
        if (IsFieldActive(fieldName)) return false;
        fieldKeys.Add(new(fieldName, withoutKey ? "" : GenerateKey()));
        return true;
    }

    public bool RemoveFieldKey(string fieldName)
    {
        for (int i = 0; i < fieldKeys.Count; i++)
        {
            if (fieldKeys[i].fieldName == fieldName)
            {
                fieldKeys.RemoveAt(i);
                return true;
            }

        }
        return false;
    }

    public FieldInfo[] GetFieldInfo()
    {
        if (target == null) return null;
        System.Type classType = target.GetType();
        fieldInfo = classType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        return fieldInfo;
    }

    public void LoadFields()
    {
        System.Type classType = target.GetType();
        for (int i = 0; i < fieldKeys.Count; i++)
        {

            FateFieldKey fieldKey = fieldKeys[i];
            FieldInfo fieldInfo = classType.GetField(fieldKey.fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo == null) continue;
            if (fieldInfo.FieldType == typeof(int))
            {
                if (SaveManager.Instance.TryGetInt(fieldKey.key, out int value))
                    fieldInfo.SetValue(target, value);
            }
            else if (fieldInfo.FieldType == typeof(float))
            {
                if (SaveManager.Instance.TryGetFloat(fieldKey.key, out float value))
                    fieldInfo.SetValue(target, value);
            }
            else if (fieldInfo.FieldType == typeof(string))
            {
                if (SaveManager.Instance.TryGetString(fieldKey.key, out string value))
                    fieldInfo.SetValue(target, value);
            }
            else if (fieldInfo.FieldType == typeof(bool))
            {
                if (SaveManager.Instance.TryGetBool(fieldKey.key, out bool value))
                    fieldInfo.SetValue(target, value);
            }
            else if (fieldInfo.FieldType == typeof(List<int>))
            {
                if (SaveManager.Instance.TryGetIntList(fieldKey.key, out List<int> value))
                    fieldInfo.SetValue(target, value);
            }
            else if (fieldInfo.FieldType == typeof(List<float>))
            {
                if (SaveManager.Instance.TryGetFloatList(fieldKey.key, out List<float> value))
                    fieldInfo.SetValue(target, value);
            }
            else if (fieldInfo.FieldType == typeof(List<string>))
            {
                if (SaveManager.Instance.TryGetStringList(fieldKey.key, out List<string> value))
                    fieldInfo.SetValue(target, value);
            }
            else if (fieldInfo.FieldType == typeof(List<bool>))
            {
                if (SaveManager.Instance.TryGetBoolList(fieldKey.key, out List<bool> value))
                    fieldInfo.SetValue(target, value);
            }
        }
    }
    public void SaveFields()
    {
        for (int i = 0; i < fieldKeys.Count; i++)
        {
            FateFieldKey fieldKey = fieldKeys[i];
            System.Type classType = target.GetType();
            FieldInfo fieldInfo = classType.GetField(fieldKey.fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo == null) continue;
            if (fieldInfo.FieldType == typeof(int))
            {
                SaveManager.Instance.SetInt(fieldKey.key, (int)fieldInfo.GetValue(target));
            }
            else if (fieldInfo.FieldType == typeof(float))
            {
                SaveManager.Instance.SetFloat(fieldKey.key, (float)fieldInfo.GetValue(target));
            }
            else if (fieldInfo.FieldType == typeof(string))
            {
                SaveManager.Instance.SetString(fieldKey.key, (string)fieldInfo.GetValue(target));
            }
            else if (fieldInfo.FieldType == typeof(bool))
            {
                SaveManager.Instance.SetBool(fieldKey.key, (bool)fieldInfo.GetValue(target));
            }
            else if (fieldInfo.FieldType == typeof(List<int>))
            {
                SaveManager.Instance.SetIntList(fieldKey.key, (List<int>)fieldInfo.GetValue(target));
            }
            else if (fieldInfo.FieldType == typeof(List<float>))
            {
                SaveManager.Instance.SetFloatList(fieldKey.key, (List<float>)fieldInfo.GetValue(target));
            }
            else if (fieldInfo.FieldType == typeof(List<string>))
            {
                SaveManager.Instance.SetStringList(fieldKey.key, (List<string>)fieldInfo.GetValue(target));
            }
            else if (fieldInfo.FieldType == typeof(List<bool>))
            {
                SaveManager.Instance.SetBoolList(fieldKey.key, (List<bool>)fieldInfo.GetValue(target));
            }
        }
    }
    protected virtual void OnValidate()
    {
        bool inPrefabMode = PrefabStageUtility.GetCurrentPrefabStage() != null || gameObject.scene.name == null;
        List<string> keys = new();
        List<string> fieldNames = new();
        for (int i = 0; i < fieldKeys.Count; i++)
        {
            FateFieldKey fieldKey = fieldKeys[i];
            if (fieldKey.fieldName != null && fieldKey.fieldName != "" && !fieldNames.Contains(fieldKey.fieldName))
                fieldNames.Add(fieldKey.fieldName);
            else if (fieldNames.Contains(fieldKey.fieldName))
                fieldKey.fieldName = "";
            if ((fieldKey.key == null || fieldKey.key == "" || keys.Contains(fieldKey.key)) && !inPrefabMode)
            {

                fieldKey.key = GenerateKey();

            }
            if (inPrefabMode) fieldKey.key = "";
            else
                keys.Add(fieldKey.key);
        }

    }

    private string GenerateKey()
    {
        string key = Guid.NewGuid().ToString();
        Debug.Log($"New key generated: {key}", this);
        return key;
    }



}

[CustomEditor(typeof(Saveable))]
public class SaveableEditor : Editor
{
    SerializedProperty saveTarget;
    Saveable saveable = null;
    private void OnEnable()
    {
        saveable = target as Saveable;
        saveTarget = serializedObject.FindProperty("target");
    }

    public override void OnInspectorGUI()
    {
        bool inPrefabMode = PrefabStageUtility.GetCurrentPrefabStage() != null || saveable.gameObject.scene.name == null;
        EditorGUILayout.PropertyField(saveTarget, new GUIContent("Target"));
        serializedObject.ApplyModifiedProperties();
        FieldInfo[] fieldInfo = saveable.GetFieldInfo();
        if (fieldInfo != null)
        {
            EditorGUILayout.Space(15);
            foreach (FieldInfo item in fieldInfo)
            {
                if (EditorGUILayout.ToggleLeft(item.Name, saveable.IsFieldActive(item.Name)))
                {
                    if (saveable.AddFieldKey(item.Name, inPrefabMode))
                        EditorUtility.SetDirty(saveable);
                }
                else
                {
                    if (saveable.RemoveFieldKey(item.Name))
                        EditorUtility.SetDirty(saveable);
                }
            }

        }

    }
}