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
using System.Management;

namespace ComandosNuevosAndy
{
    public class NombreComandoLineaParalelaA
    {
        // Creación del nombre del comando nuevo para AutoCAD
        [CommandMethod("ACL_LineParalelaA")]
        // Creamos un Public Void para crear un Comando Nuevo en AutoCAD
        // Cada Public void, es un comando separado de AutoCAD
        public void ACL_LineParalelaA()
        {
            // Obtienemos una Instancia del Documento y Solicita el acceso a un Documento activo de AutoCAD
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            // Obtienemos una Instancia de la Base de Datos y Solicita el acceso a la Base de Datos de AutoCAD
            Database acCurDb = acDoc.Database;
            // Solicita el acceso al Editor de AutoCAD
            Editor acEditor = acDoc.Editor;

            // ::::::: Apartir de aquí se codifica el Comando Nuevo :::::::
            // Inicia la Transación para realizar cambios en la Base de Datos de AutoCAD
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
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

                // Validar la respuesta del usuario para el primer punto
                if (acPuntoA.Status != PromptStatus.OK)
                {
                    acEditor.WriteMessage("\n---> No se clickó el primer punto A:");
                    return;
                }
                if (acPuntoA.Status == PromptStatus.OK)
                {
                    // Opciones del segundo punto
                    PromptPointOptions optsPuntoB = new PromptPointOptions("\n---> Clickar el segundo punto B: ");
                    optsPuntoB.UseBasePoint = true;
                    optsPuntoB.BasePoint = acPuntoA.Value;
                    optsPuntoB.UseDashedLine = true;
                    PromptPointResult acPuntoB = acEditor.GetPoint(optsPuntoB);

                    // Validar la respuesta del usuario para el segundo punto
                    if (acPuntoB.Status != PromptStatus.OK)
                    {
                        acEditor.WriteMessage("\n---> No se clickó el segundo punto B");
                        return;
                    }
                    if (acPuntoB.Status == PromptStatus.OK)
                    {
                        // Solicitar la distancia de desplazamiento
                        PromptDoubleOptions optsDistancia = new PromptDoubleOptions("\n---> Ingresar la distancia para la línea paralela: ");
                        optsDistancia.AllowNegative = true;
                        optsDistancia.AllowZero = false;
                        PromptDoubleResult Distancia = acEditor.GetDouble(optsDistancia);

                        // Validar la respuesta del usuario para la distancia
                        if (Distancia.Status != PromptStatus.OK)
                        {
                            acEditor.WriteMessage("\n---> No se ingresó una distancia válida:");
                            return;
                        }
                        if (Distancia.Status == PromptStatus.OK)
                        {
                            // En este espacio se procede a crear los objetos de AutoCAD

                            // Crear los puntos A y B
                            Point3d puntoA = acPuntoA.Value;
                            Point3d puntoB = acPuntoB.Value;

                            // (1) Crear el vector de la línea original
                            Vector3d vectorAB = puntoB - puntoA;

                            // (2) Hallamos su magnitud y el vector unitario
                            double magnitudAB = Math.Sqrt(puntoB.X - puntoA.X * puntoB.X - puntoA.X + puntoB.Y - puntoA.Y * puntoB.Y - puntoA.Y);
                            double unitarioX = (puntoB.X - puntoA.X) / magnitudAB;
                            double unitarioY = (puntoB.Y - puntoA.Y) / magnitudAB;

                            // (3) Crear el vector perpendicular en el plano XY
                            Vector3d vectorPerpendicular = new Vector3d(-unitarioY, unitarioX, 0);

                            // (4) Escalar el vector perpendicular por la distancia deseada
                            double distanciaDesplazadaX = vectorPerpendicular.X * Distancia.Value;
                            double distanciaDesplazadaY = vectorPerpendicular.Y * Distancia.Value;

                            // (5) Crear los nuevos puntos para la línea paralela
                            Point2d nuevoPuntoA = new Point2d(puntoA.X + distanciaDesplazadaX, puntoA.Y + distanciaDesplazadaY);
                            Point2d nuevoPuntoB = new Point2d(puntoB.X + distanciaDesplazadaX, puntoB.Y + distanciaDesplazadaY);

                            // (6) Crear la línea base y paralela en AutoCAD
                            Line lineaBase = new Line(puntoA, puntoB);
                            Line lineaParalela = new Line();
                            lineaParalela.StartPoint = new Point3d(nuevoPuntoA.X, nuevoPuntoA.Y, 0);
                            lineaParalela.EndPoint = new Point3d(nuevoPuntoB.X, nuevoPuntoB.Y, 0);

                            // Añadir las líneas al espacio modelo
                            acBlkTblRec.AppendEntity(lineaBase);
                            acTrans.AddNewlyCreatedDBObject(lineaBase, true);
                            acBlkTblRec.AppendEntity(lineaParalela);
                            acTrans.AddNewlyCreatedDBObject(lineaParalela, true);
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