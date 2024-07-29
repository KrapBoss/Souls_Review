using System;
using System.IO;

/// <summary>
/// ���� ���� Ŭ����
/// </summary>
public static class PlayerPrefs // UnityEngine.PlayerPrefs Overriding
{
    /// <summary>
    /// ������ �ҷ�����
    /// </summary>
    /// <typeparam name="T">������ Ÿ��</typeparam>
    /// <param name="name">�̸�</param>
    /// <returns>������ ��ȯ</returns>
    public static T Load<T>(string name, T defaultValue = default)
    {
        //������ Ű�� ���ٸ� ���� �� �ҷ�����
        if (!RegisterKey(name)) Save(name, defaultValue);

        T data = FileLoad<T>(name);

        return data;
    }

    /// <summary>
    /// ������ �����ϱ�
    /// </summary>
    /// <typeparam name="T">������ Ÿ��</typeparam>
    /// <param name="name">�̸�</param>
    /// <param name="data">������</param>
    public static void Save<T>(string name, T data)
    {
        FileSave(name, data);
    }

    /// <summary>
    /// Ű ��� ����
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private static bool RegisterKey(string key) => HasKey(key);

    #region ## File I/O ##
    /// <summary>
    /// ���� ������ �ҷ�����
    /// </summary>
    /// <typeparam name="T">������ Ÿ��</typeparam>
    /// <param name="name">�̸�</param>
    /// <returns>������ ��ȯ</returns>
    private static T FileLoad<T>(string name)
    {
        if (UnityEngine.PlayerPrefs.HasKey(name))
        {
            var loadData = UnityEngine.PlayerPrefs.GetString(name);
            if (loadData != null)
            {
                byte[] bytes = Convert.FromBase64String(loadData);
                using (var ms = new MemoryStream(bytes))
                {
                    object obj = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Deserialize(ms);
                    return (T)obj;
                }
            }
        }
        return default;
    }

    /// <summary>
    /// ���� ������ ����
    /// </summary>
    /// <typeparam name="T">������ Ÿ��</typeparam>
    /// <param name="name">�̸�</param>
    /// <param name="data">������</param>
    private static void FileSave<T>(string name, T data)
    {
        if (data != null)
        {
            using (var ms = new MemoryStream())
            {
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(ms, data);
                UnityEngine.PlayerPrefs.SetString(name, Convert.ToBase64String(ms.ToArray()));
            }
        }
        else
        {
            UnityEngine.PlayerPrefs.SetString(name, null);
        }

        UnityEngine.PlayerPrefs.Save();
    }
    #endregion

    #region ## PlayerPrefs original method ##
    public static void SetInt(string key, int value) => Save(key, value);
    public static int GetInt(string key, int defaultValue = 0) => Load(key, defaultValue);
    public static void SetFloat(string key, float value) => Save(key, value);
    public static float GetFloat(string key, float defaultValue = 0f) => Load(key, defaultValue);
    public static void SetString(string key, string value) => Save(key, value);
    public static string GetString(string key, string defaultValue = "") => Load(key, defaultValue);
    public static bool HasKey(string key) => UnityEngine.PlayerPrefs.HasKey(key);
    public static void DeleteKey(string key)
    {
        UnityEngine.PlayerPrefs.DeleteKey(key);
    }
    public static void DeleteAll()
    {
        UnityEngine.PlayerPrefs.DeleteAll();
    }

    public static void Save() => UnityEngine.PlayerPrefs.Save();

    #endregion
}