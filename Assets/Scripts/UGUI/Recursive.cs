//==========================================
// Created By [lijun2] At 2015/5/5 10:43:37
//==========================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 公用递归遍历类
/// </summary>
public class Recursive 
{
    ///////////////////////////////////以下为静态成员//////////////////////////////
    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();

    ///////////////////////////////////以下为非静态成员///////////////////////////

    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    /// <param name="path">遍历目录</param>
    /// <param name="incExts">包含扩展名数组</param>
    /// <param name="expExts">排除扩展名数组</param>
    public static void Start(string path, string incExts = null, string expExts = null)
    {
        paths.Clear();    //清理路径
        files.Clear();    //清理文件
        List<string> includes = null;
        if (incExts != null && incExts.Length > 0)
        {
            includes = new List<string>(incExts.Split('|'));
        }
        List<string> excepts = null;
        if (expExts != null && expExts.Length > 0)
        {
            excepts = new List<string>(expExts.Split('|'));
        }
        RunProc(path, includes, excepts);
    }

    /// <summary>
    /// 开始递归
    /// </summary>
    /// <param name="path">遍历目录</param>
    /// <param name="incExts">包含扩展名数组</param>
    /// <param name="expExts">排除扩展名数组</param>
    public static void RunProc(string path, List<string> incExts = null, List<string> expExts = null)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (incExts != null)
            {
                if (-1 == incExts.IndexOf(ext))
                {
                    continue;
                }
            }
            else if (expExts != null)
            {
                if (-1 < expExts.IndexOf(ext))
                {
                    continue;
                }
            }
            files.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            RunProc(dir, incExts, expExts);
        }
    }

    /// <summary>
    /// 获取递归后所有文件列表
    /// </summary>
    /// <returns></returns>
    public static List<string> GetFiles()
    {
        return files;
    }

    /// <summary>
    /// 获取递归后所有路径
    /// </summary>
    /// <returns></returns>
    public static List<string> GetPaths()
    {
        return paths;
    }
}
