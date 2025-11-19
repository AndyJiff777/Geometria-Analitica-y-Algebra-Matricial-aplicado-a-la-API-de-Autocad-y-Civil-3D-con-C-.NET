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
    public class NombreComandoCompHorz2
    {
        // Creación del nombre del comando nuevo para AutoCAD
        [CommandMethod("ACL_CompHorz2")]
        // Creamos un Public Void para crear un Comando Nuevo en AutoCAD
        // Cada Public void, es un comando separado de AutoCAD
        public void ACL_CompHorz2()
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

                        // (1) Convertir los puntos 3D a 2D (ignorando la coordenada Z)
                        Point2d puntoA2D = new Point2d(acPuntoA.Value.X, acPuntoA.Value.Y);
                        Point2d puntoB2D = new Point2d(acPuntoB.Value.X, acPuntoB.Value.Y);

                        // (2) Creamos el vector desde el punto A hasta el punto B
                        Vector2d AB = puntoB2D-puntoA2D;
                        double magnitudAB = AB.Length;
                        // Obtenemos el Vector Ortogonal en el plano XY
                        AB.GetPerpendicularVector();

                        // (3) Calcular el ángulo de inclinación en Radianes usando atan2
                        double anguloEnRadianes = Math.Atan2(AB.Y,AB.X);

                        // (4) Convertir el ángulo a grados
                        double anguloEnGrados = anguloEnRadianes * (180.0 / Math.PI);

                        // Crear el punto A y B en AutoCAD
                        DBPoint puntoA = new DBPoint(acPuntoA.Value);
                        DBPoint puntoB = new DBPoint(acPuntoB.Value);

                        // Creamos el punto del vector perpendicular en AutoCAD
                        DBPoint puntoPerpendicular = new DBPoint(new Point3d(acPuntoA.Value.X + AB.GetPerpendicularVector().X, acPuntoA.Value.Y + AB.GetPerpendicularVector().Y, 0));

                        Line acLine = new Line();
                        acLine.StartPoint = acPuntoB.Value;

                        // Agregamos los puntos a la Base de Datos de AutoCAD
                        acBlkTblRec.AppendEntity(puntoA);
                        acTrans.AddNewlyCreatedDBObject(puntoA, true);
                        acBlkTblRec.AppendEntity(puntoB);
                        acTrans.AddNewlyCreatedDBObject(puntoB, true);
                        acBlkTblRec.AppendEntity(puntoPerpendicular);
                        acTrans.AddNewlyCreatedDBObject(puntoPerpendicular, true);

                        // Mostrar el resultado
                        acEditor.WriteMessage($"\nEl ángulo de inclinación del radio vector es: {anguloEnGrados} grados.");
                        acEditor.WriteMessage($"\nLa magnitud del vector es: {magnitudAB} unidades.");
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