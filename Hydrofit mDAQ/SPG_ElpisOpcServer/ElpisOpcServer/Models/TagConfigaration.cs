using Elpis.Windows.OPC.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElpisOpcServer.Models
{
    public class TagConfigaration
    {
       
        
        public string TagName;
        public string SignalId;
        public string Description;
        public string Address;
        public int MinValue;
        public int MaxValue;
        public int Divisions;
        public Units Units;
        public BlockTypes BlockType;
        public DataType DataType;


    }
}
