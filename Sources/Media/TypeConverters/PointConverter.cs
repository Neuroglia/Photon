﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photon.Media
{

    public class PointConverter
        : TypeConverter
    {

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (typeof(string).IsAssignableFrom(sourceType))
            {
                return true;
            }
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str;
            string[] temp;
            double x, y;
            str = (string)value;
            temp = str.Replace(" ", "").Split(',');
            if (temp.Length != 2)
            {
                throw new Exception("The specified string '" + str + "' cannot be parsed into a instance of the Point type");
            }
            if(!double.TryParse(temp[0], out x))
            {
                throw new Exception("The specified string '" + str + "' cannot be parsed into a instance of the Point type");
            }
            if (!double.TryParse(temp[1], out y))
            {
                throw new Exception("The specified string '" + str + "' cannot be parsed into a instance of the Point type");
            }
            return new Point(x, y);
        }

    }

}
