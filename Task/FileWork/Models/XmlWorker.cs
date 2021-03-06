﻿using System;
using System.Collections.Generic;
using System.Xml;
using Task.Enums;
using Task.Figures.Interfaces;
using Task.FileWork.Interfaces;
using Task.MyParsers.Interfaces;
using Task.MyParsers.Models;

namespace Task.FileWork.Models
{
    /// <summary>
    /// Class for working with xml file via read and write through XmlReader and XmlWriter
    /// </summary>
    public class XmlWorker : IXmlFileWorker
    {
        private static readonly IPaperFiguresParser _paperFiguresParser = new XmlPaperFiguresParser();
        private static readonly IPaperXmlElementWriter _paperXmlElementWriter = new XmlPaperFiguresWriter();
        private static readonly IFilmFiguresParser _filmFiguresParser = new XmlFilmFiguresParser();
        private static readonly IFilmXmlElementWriter _filmXmlElementWriter = new XmlFilmFiguresWriter();

        /// <inheritdoc cref="IXmlFileWorker.ReadFiguresFromXML(string, XmlReader)"/>
        public IEnumerable<IFigure> ReadFiguresFromXML(string path, XmlReader reader)
        {
            List<IFigure> figures = new List<IFigure>();
            using (reader = new XmlTextReader(path))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement() && reader.Name != "Figures")
                    {
                        Enum.TryParse(reader.Name, out FigureTypes currentType);
                        switch (currentType)
                        {
                            case FigureTypes.PaperCircle: figures.Add(_paperFiguresParser.ParseToPaperCircleFromXml(reader)); break;
                            case FigureTypes.PaperRectangle: figures.Add(_paperFiguresParser.ParseToPaperRectangleFromXml(reader)); break;
                            case FigureTypes.PaperSquare: figures.Add(_paperFiguresParser.ParseToPaperSquareFromXml(reader)); break;
                            case FigureTypes.PaperTriangle: figures.Add(_paperFiguresParser.ParseToPaperTriangleFromXml(reader)); break;
                            case FigureTypes.FilmCircle: figures.Add(_filmFiguresParser.ParseToFilmCircleFromXml(reader)); break;
                            case FigureTypes.FilmRectangle: figures.Add(_filmFiguresParser.ParseToFilmRectangleFromXml(reader)); break;
                            case FigureTypes.FilmSquare: figures.Add(_filmFiguresParser.ParseToFilmSquareFromXml(reader)); break;
                            case FigureTypes.FilmTriangle: figures.Add(_filmFiguresParser.ParseToFilmTriangleFromXml(reader)); break;
                        }
                    }
                }
            }
            return figures;
        }

        /// <inheritdoc cref="IXmlFileWorker.WriteFiguresFromBoxToXML(string, IEnumerable{IFigure}, XmlWriter)"/>
        public bool WriteFiguresFromBoxToXML(string path, IEnumerable<IFigure> figures, XmlWriter writer)
        {
            using (writer = XmlWriter.Create(path))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Figures");
                foreach (IFigure figure in figures)
                {
                    Enum.TryParse(figure.GetType().Name, out FigureTypes currentType);
                    switch (currentType)
                    {
                        case FigureTypes.PaperCircle: _paperXmlElementWriter.WriteCircularPaperFigure(writer, figure as IPaperFigure); break;
                        case FigureTypes.PaperRectangle:
                        case FigureTypes.PaperSquare:
                        case FigureTypes.PaperTriangle: _paperXmlElementWriter.WritePolygonPaperFigure(writer, figure as IPaperFigure); break;
                        case FigureTypes.FilmCircle: _filmXmlElementWriter.WriteCircularFilmFigure(writer, figure as IFilmFigure); break;
                        case FigureTypes.FilmRectangle:
                        case FigureTypes.FilmSquare:
                        case FigureTypes.FilmTriangle: _filmXmlElementWriter.WritePolygonFilmFigure(writer, figure as IFilmFigure); break;
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                return true;
            }
        }
    }
}