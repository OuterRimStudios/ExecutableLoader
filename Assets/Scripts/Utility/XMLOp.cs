using System.IO;
using System.Xml.Serialization;

public class XMLOp {

    /// <summary> Saves data from an item to a file</summary>
    /// <param name="item"> object to write</param>
    /// <param name="path">filename</param>
	public static void Serialize(object item, string path)
    {
        XmlSerializer serializer = new XmlSerializer(item.GetType());
        StreamWriter writer = new StreamWriter(path);
        serializer.Serialize(writer.BaseStream, item);
        writer.Close();
    }

    /// <summary>Reads data from a file</summary>
    /// <typeparam name="T">Type of object that is stored</typeparam>
    /// <param name="path">filename</param>
    /// <returns>Object of specified type with data read from file</returns>
    public static T Deserialize<T> (string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StreamReader reader = new StreamReader(path);
        T deserialized = (T)serializer.Deserialize(reader.BaseStream);
        reader.Close();
        return deserialized;
    }
}