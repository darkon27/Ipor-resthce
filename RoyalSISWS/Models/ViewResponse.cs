﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoyalSISWS.Models
{
    public class ViewResponse
    {
        public Boolean ok { get; set; }
        public Object msg { get; set; }
        public Nullable<int> valor { get; set; }
        public string tokem { get; set; }

    }

    public class ViewResponseContenido
    {
        public Boolean ok { get; set; }
        public Object msg { get; set; }
        public Object contenido { get; set; }
        public Nullable<int> valor { get; set; }
        public string tokem { get; set; }

    }

    public class ViewResponseContenidoPaginado
    {
        public Boolean ok { get; set; }
        public Object msg { get; set; }
        public Object contenido { get; set; }
        public Nullable<int> valor { get; set; }
        public string tokem { get; set; }
        public int inicio { get; set; }
        public int total { get; set; }



    }
}