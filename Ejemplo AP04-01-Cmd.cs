using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.LayerManager;
using Autodesk.AutoCAD.Colors;
using System.Collections;
using System.Linq.Expressions;
using System.Windows.Forms;
using System;

namespace ComandosNuevosAndy
{
    public class NombreComandoEjemploAP04
    {
        // Creación del nombre del comando nuevo para AutoCAD
        [CommandMethod("ACL_AP04")]
        // Creamos un Public Void para crear un Comando Nuevo en AutoCAD
        // Cada Public void, es un comando separado de AutoCAD
        public void ACL_AP04()
        {
            // Obtienemos una Instancia del Documento y Solicita el acceso a un Documento activo de AutoCAD
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            // Obtienemos una Instancia de la Base de Datos y Solicita el acceso a la Base de Datos de AutoCAD
            Database acCurDb = acDoc.Database;
            // Solicita el acceso al Editor de AutoCAD
            Editor acEditor = acDoc.Editor;

            // ::::::: Apartir de aquí se codifica el Comando Nuevo :::::::
            // Inicia la Transación para realizar cambios en la Base de Datos de AutoCAD
            using (Transaction acTrans=acCurDb.TransactionManager.StartTransaction())
            {
                // Abre el Diccionario de objetos actual de AutoCAD
                BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                // Abre el espacio modelo (ModelSpace)
                BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Desde aquí se realiza las distintas operaciones y creación de objetos
                // Aquí inicia la creación del comando nuevo

                // Opciones del primer punto
                PromptPointOptions optsPuntoA = new PromptPointOptions("\n---> Clickar el primer punto A: ");
                // Capturamos el punto de la pantalla de AutoCAD
                PromptPointResult acPuntoA = acEditor.GetPoint(optsPuntoA);
                // Validar la respuesta del usuario
                if (acPuntoA.Status!=PromptStatus.OK)
                {
                    acEditor.WriteMessage("\n---> No se clickó el primer punto A:");
                    return;
                }
                if (acPuntoA.Status==PromptStatus.OK)
                {
                    // Opciones del segundo punto
                    PromptPointOptions optsPuntoB = new PromptPointOptions("\n---> Clickar el segundo punto B: ");
                    optsPuntoB.UseBasePoint = true;
                    optsPuntoB.BasePoint = acPuntoA.Value;
                    optsPuntoB.UseDashedLine = true;
                    PromptPointResult acPuntoB = acEditor.GetPoint(optsPuntoB);

                    // Validar la respuesta del usuario
                    if(acPuntoB.Status!=PromptStatus.OK)
                    {
                        acEditor.WriteMessage("\n---> No se clickó el segundo punto B");
                        return;
                    }
                    if (acPuntoB.Status==PromptStatus.OK)
                    {
                        // En este espacio se procede a crear los objetos de AutoCAD
                        PromptDistanceOptions optsDistancia = new PromptDistanceOptions("\n---> Ingresa la distancia para el puntp P: ");
                        optsDistancia.AllowNone = false;
                        optsDistancia.UseDefaultValue = true;
                        PromptDoubleResult acDistancia = acEditor.GetDistance(optsDistancia);
                        optsDistancia.AllowNegative = true;
                        optsDistancia.DefaultValue = 10;
                        optsDistancia.AllowZero = false;
                        // Validar la respuesta del usuario
                        if (acDistancia.Status!=PromptStatus.OK)
                        {
                            acEditor.WriteMessage("\n---> No se ingresó la distancia para el punto P: ");
                            return;
                        }
                        if (acDistancia.Status==PromptStatus.OK)
                        {
                            // (1) Crear el vector AB
                            Vector3d vectorAB= acPuntoB.Value - acPuntoA.Value;
                            double xD = vectorAB.X;
                            double yD = vectorAB.Y;

                            // (2) Hallar la longitud del vector AB
                            double magnitudAB = Math.Sqrt(xD*xD+yD*yD);

                            // (3) Crear el vector unitario uAB
                            double uX = xD/magnitudAB;
                            double uY = yD/magnitudAB;

                            // (4) Calcular las coordenadas del punto P
                            double xP = acPuntoB.Value.X + uX * acDistancia.Value;
                            double yP = acPuntoB.Value.Y + uY * acDistancia.Value;
                            Point3d puntoP = new Point3d(xP, yP, 0);

                            // Agregar los objetos al espacio modelo
                            // Crear la línea desde el punto A hasta el punto B
                            Line lineaAB = new Line(acPuntoA.Value, acPuntoB.Value);
                            lineaAB.Color = Color.FromColorIndex(ColorMethod.ByAci, 10); // Color rojo
                            acBlkTblRec.AppendEntity(lineaAB);
                            acTrans.AddNewlyCreatedDBObject(lineaAB, true);
                            // Crear el punto A, B y P
                            DBPoint puntoDBA = new DBPoint(acPuntoA.Value);
                            puntoDBA.Color = Color.FromColorIndex(ColorMethod.ByAci, 10); // Color rojo
                            acBlkTblRec.AppendEntity(puntoDBA);
                            acTrans.AddNewlyCreatedDBObject(puntoDBA, true);
                            DBPoint puntoDBB = new DBPoint(acPuntoB.Value);
                            puntoDBB.Color = Color.FromColorIndex(ColorMethod.ByAci, 10); // Color rojo
                            acBlkTblRec.AppendEntity(puntoDBB);
                            acTrans.AddNewlyCreatedDBObject(puntoDBB, true);
                            DBPoint puntoDBP = new DBPoint(puntoP);
                            puntoDBP.ColorIndex = 5; // Color azul
                            acBlkTblRec.AppendEntity(puntoDBP);
                            acTrans.AddNewlyCreatedDBObject(puntoDBP, true);

                            // Mensajes en la línea de comandos de AutoCAD
                            acEditor.WriteMessage($"\n---> Punto P creado con una distancia: {acDistancia.Value}; resultando las coordenadas: X: {xP} | Y: {yP}");
                        }
                    }
                }
                // Aquí termina la creación del comando nuevo
                // Hasta aquí se realiza las distintas operaciones y creación de objetos

                // Cierra la Transacción y guarda los cambios en la Base de Datos de AutoCAD
                acTrans.Commit();
            }
            // ::::::: Hasta aquí se codifica el Comando Nuevo :::::::
        }
        //Aquí termina el Public Void, es decir el Comando Nuevo
    }
}