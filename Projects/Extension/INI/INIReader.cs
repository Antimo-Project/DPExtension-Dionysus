﻿using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.INI
{
    public class INIReader
    {
        Pointer<CCINIClass> IniFile;
        byte[] readBuffer = new byte[2048];

        public INIReader(Pointer<CCINIClass> pINI)
        {
            IniFile = pINI;
        }

        public Encoding Encoding { get; set; } = Encoding.UTF8;


        /// <summary>
        /// read data into buffer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="buffer"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public bool Read<T>(string section, string key, ref T buffer, IParser<T> parser = null)
        {
            parser ??= Parsers.GetParser<T>();

            if (ReadString(section, key, out string val))
            {
                return parser.Parse(val, ref buffer);
            }

            return false;
        }

        /// <summary>
        /// read data into array array.Length times
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="buffer"></param>
        /// <param name="count">the count of values to read into buffer. -1 mean the length of buffer</param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public bool ReadArray<T>(string section, string key, ref T[] buffer, int count = -1, IParser<T> parser = null)
        {
            if (count == -1)
            {
                count = buffer.Length;
            }
            parser ??= Parsers.GetParser<T>();

            if (ReadString(section, key, out string val))
            {
                return parser.ParseArray(val, ref buffer, count);
            }
            return false;
        }

        /// <summary>
        /// read data into list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="list"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public bool ReadList<T>(string section, string key, ref List<T> buffer, IParser<T> parser = null)
        {
            parser ??= Parsers.GetParser<T>();

            if (ReadString(section, key, out string val))
            {
                return parser.ParseList(val, ref buffer);
            }

            return false;
        }



        private string GetString()
        {
            string str = Encoding.GetString(readBuffer);
            str = str.Substring(0, str.IndexOf('\0'));
            str = str.Trim();

            return str;
        }
        private int ReadBuffer(string section, string key)
        {
            return IniFile.Ref.ReadString(section, key, "", readBuffer, readBuffer.Length);
        }
        private bool ReadString(string section, string key, out string buffer)
        {
            if (ReadBuffer(section, key) > 0)
            {
                buffer = GetString();
                return true;
            }

            buffer = null;
            return false;
        }
    }
}
