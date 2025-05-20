using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml;
using RimWorld_Mod_Structure_Builder.InfoClasses;
using RimWorld_Mod_Structure_Builder.InfoClasses.Interfaces;

namespace RimWorld_Mod_Structure_Builder.Utils;

public static class XmlUtils
{
    public static bool AreXmlDocumentsEqual(XmlDocument doc1, XmlDocument doc2)
    {
        if (doc1 == null && doc2 == null)
            return true;
        if (doc1 == null || doc2 == null)
            return false;
        return doc1.OuterXml.Equals(doc2.OuterXml, StringComparison.Ordinal);
    }
    
    private static string GetVersion<TSource>(this TSource source) =>
        source?.GetType().GetProperty("Version")?.GetValue(source)?.ToString();

    public static void AddSimpleNode(XmlElement parent, string name, string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        
        var node = parent.OwnerDocument?.CreateElement(name);
        if (node == null) return;

        node.InnerText = value;
        parent.AppendChild(node);
    }

    public static void AddListNode<T>(XmlElement parent, string singleName, string listName, IEnumerable<T> inputValues)
    {
        if (inputValues == null) return;
        var values = inputValues.ToList();
        
        if (!values.Any()) return;
        var ownerDocument = parent.OwnerDocument;
        if (ownerDocument == null) return;

        if (values.Count() == 1)
        {
            AddSimpleNode(parent, singleName, values.First().ToString());
        }
        else
        {
            AddListNode(parent, listName, values);
        }
    }

    public static void AddListNode<T>(XmlElement parent, string listName, IEnumerable<T> inputValues)
    {
        if (inputValues == null) return;
        var values = inputValues.ToList();
        
        if (!values.Any()) return;
        var ownerDocument = parent.OwnerDocument;
        if (ownerDocument == null) return;

        var listNode = ownerDocument.CreateElement(listName);
        parent.AppendChild(listNode);

        foreach (var value in values)
        {
            var node = ownerDocument.CreateElement("li");
            node.InnerText = value.ToString();
            listNode.AppendChild(node);
        }
    }

    public static void AddListVersionNode<T>(XmlElement parent, string singleName, string listName,
        Func<T, string> versionSelector, Func<T, string> valueSelect, IEnumerable<T> inputValues)
    {
        if (inputValues == null) return;
        var values = inputValues.ToList();
        
        if (!values.Any()) return;
        var ownerDocument = parent.OwnerDocument;
        if (ownerDocument == null) return;

        if (values.Count() == 1)
        {
            AddSimpleNode(parent, singleName, valueSelect(values.First()));
        }
        else
        {
            var listNode = ownerDocument.CreateElement(listName);

            foreach (var value in values)
            {
                var version = versionSelector(value);
                if (string.IsNullOrEmpty(version)) continue;
                
                var versionNode = ownerDocument.CreateElement(version);
                versionNode.InnerText = valueSelect(value);
                listNode.AppendChild(versionNode);
            }
        }
    }

    /// <summary>
    /// Adds the specified list of objects to the parent node.
    /// </summary>
    /// <param name="parent">The parent node to add the list to.</param>
    /// <param name="listName">The name of the list node to add.</param>
    /// <param name="inputData">The list of objects to add.</param>
    /// <param name="selector">A function that selects the data to add from the objects in the list.</param>
    /// <remarks>
    /// If any of the objects in <paramref name="inputData"/> have a non-null version, a node with the name
    /// <paramref name="listName"/>ByVersion is added to <paramref name="parent"/>, and the objects in <paramref name="inputData"/>
    /// are added to that node. Otherwise, the objects are added to a node with the name <paramref name="listName"/>.
    /// </remarks>
    public static void AddByVersionNode<TSource, TResult>(XmlElement parent, string listName, IEnumerable<TSource> inputData,
        Func<TSource, TResult> selector)
    {
        if (inputData == null) return;
        var data = inputData.ToList();
        
        if (!data.Any()) return;
        var ownerDocument = parent.OwnerDocument;
        if (ownerDocument == null) return;

        // Get the selected data
        var selectedData = data.Select(selector).ToList();
        if (!selectedData.Any()) return;

        var listNode = ownerDocument.CreateElement(listName);
        var isVersioned = selectedData.Any(d => d.GetVersion() != null);
        XmlElement versionedListNode = null;

        if (isVersioned)
            versionedListNode = ownerDocument.CreateElement($"{listName}ByVersion");

        parent.AppendChild(isVersioned ? versionedListNode : listNode);

        foreach (var dataItem in selectedData)
        {
            var node = ownerDocument.CreateElement(dataItem.GetVersion() ?? "li");
            
            // Single property (<li>value</li>)
            if (dataItem.GetType().GetProperties().Count(p => p.Name != "Version") <= 1)
                node.InnerText = dataItem.GetType().GetProperties().First(p => p.Name != "Version").GetValue(dataItem)?.ToString() ?? string.Empty;
            // Multiple properties (<li>name=value</li>)
            else
                foreach (var propertyInfo in dataItem.GetType().GetProperties().Where(p => p.Name != "Version"))
                    AddSimpleNode(node, propertyInfo.Name, propertyInfo.GetValue(dataItem)?.ToString());

            (dataItem.GetVersion() == null ? listNode : versionedListNode)?.AppendChild(node);
        }
    }
}