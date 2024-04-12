//Written for Generation Streets. https://store.steampowered.com/app/887970
using System.IO;
using System.IO.Compression;

namespace Generation_Streets_Extractor
{
    class Program
    {
        static BinaryReader br;
        static void Main(string[] args)
        {
            FileStream input = File.OpenRead(args[0]);
            br = new BinaryReader(input);
            br.BaseStream.Position = 52;

            int fileCount = br.ReadInt32();
            br.BaseStream.Position = 128;

            System.Collections.Generic.List<Subfile> subfiles = new();
            for (int i = 0; i < fileCount; i++)
                subfiles.Add(new());

            long fileData = br.BaseStream.Position;
            int n = 0;
            Directory.CreateDirectory(Path.GetDirectoryName(input.Name) + "//" + Path.GetFileNameWithoutExtension(input.Name));
            foreach (Subfile sub in subfiles)
            {
                br.BaseStream.Position = sub.start + fileData;
                BinaryWriter bw = new(File.Create(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + "//" + n));
                if (sub.isCompressed == 1)
                {
                    MemoryStream ms = new();
                    br.ReadInt16();
                    using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes(sub.sizeCompressed)), CompressionMode.Decompress))
                        ds.CopyTo(ms);
                    br = new(ms);
                    br.BaseStream.Position = 0;
                    bw.Write(br.ReadBytes(sub.sizeUncompressed));
                    bw.Close();
                    br = new(input);
                    n++;
                    continue;
                }
                else if (sub.isCompressed == 2)
                {
                    bw.Write(br.ReadBytes(sub.sizeCompressed));
                    bw.Close();
                    n++;
                    continue;
                }
                bw.Write(br.ReadBytes(sub.sizeUncompressed));
                bw.Close();
                n++;
            }
        }

        public class Subfile
        {
            public long start = br.ReadInt64();
            public int sizeCompressed = br.ReadInt32();
            public int sizeUncompressed = br.ReadInt32();
            public long isCompressed = br.ReadInt64();
        }
    }
}
