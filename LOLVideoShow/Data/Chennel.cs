using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using LOLVideoShow.Data;

namespace LOLVideoShow.Class
{
    public class Chennel
    {

        public static ChennelInfo From(MatchInfo info)
        {
            if (info == null) return null;
            return setChennel(info.id, info.name, ChennelType.Match, info.info);
        }

        public static ChennelInfo From(HeroInfo info)
        {
            if (info == null) return null;
            return setChennel(info.id, info.name, ChennelType.Hero, info.info);
        }

        public static ChennelInfo From(JieshuoInfo info)
        {
            if (info == null) return null;
            return setChennel(info.id, info.name, ChennelType.Jieshuo, info.info);
        }

        public static ChennelInfo setChennel(int id, string title, ChennelType mod, string info = "")
        {
            return new ChennelInfo()
            {
                id = id,
                title = title,
                mod = mod,
                info = info,
            };
        }

        public static ChennelType getChennelType(string str)
        {
            switch (str)
            {
                case "hero":
                    return ChennelType.Hero;
                case "jieshuo":
                    return ChennelType.Jieshuo;
                case "match":
                    return ChennelType.Match;
            }
            return ChennelType.Null;
        }

    }
}
