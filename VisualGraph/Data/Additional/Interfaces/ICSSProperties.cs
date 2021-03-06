﻿using System;
using System.Collections.Generic;

namespace VisualGraph.Data.Additional.Interfaces
{
    public interface ICSSProperties
    {
        public string ClassesProppertie => String.Join(' ', Classes );
        public List<string> Classes { get; set; }
        public string Id { get; set; }
    }
}