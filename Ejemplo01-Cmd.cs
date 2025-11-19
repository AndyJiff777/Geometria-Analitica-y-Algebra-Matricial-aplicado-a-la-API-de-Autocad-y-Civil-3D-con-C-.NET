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
    public class NombreComandosNuevos02
    {
        // Creación del nombre del comando nuevo para AutoCAD
        [CommandMethod("ACL_CompHorz1")]
        // Creamos un Public Void para crear un Comando Nuevo en AutoCAD
        // Cada Public void, es un comando separado de AutoCAD
        public void ACL_CompHorz1()
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
                PromptPointOptions optsPuntoA = new PromptPointOptions("\n---> Clickar el punto A: ");
                optsPuntoA.UseBasePoint = true;
                optsPuntoA.BasePoint = Point3d.Origin;
                optsPuntoA.UseDashedLine = true;
                // Capturamos el punto de la pantalla de AutoCAD
                PromptPointResult acPuntoA = acEditor.GetPoint(optsPuntoA);
                // Validar la respuesta del usuario
                if (acPuntoA.Status!=PromptStatus.OK)
                {
                    acEditor.WriteMessage("\n---> No se clickó el punto A:");
                    return;
                }
                if (acPuntoA.Status==PromptStatus.OK)
                {
                    // (1) Creamos el punto A en 2D
                    Point2d A2D = new Point2d(acPuntoA.Value.X, acPuntoA.Value.Y);

                    // (2) Creamos el radio vector desde el origen hasta el punto A
                    Vector2d radioVector = A2D - Point2d.Origin;
                    // Obtener la longitud del radio vector
                    double longitudRadioVector = radioVector.Length;
                    // Creamos un vector ortogonal en el plano XY
                    radioVector.GetPerpendicularVector();

                    // (3) Calcular el ángulo de inclinación usando atan2
                    double anguloEnRadianes = Math.Atan2(radioVector.Y, radioVector.X);

                    // (4) Convertir el ángulo a grados
                    double anguloEnGrados = anguloEnRadianes * (180.0 / Math.PI);

                    // Crear el punto A en AutoCAD
                    DBPoint puntoA = new DBPoint(acPuntoA.Value);

                    // Crear el punto del vector ortogonal en AutoCAD
                    DBPoint puntoOrtogonal = new DBPoint(new Point3d(acPuntoA.Value.X + radioVector.GetPerpendicularVector().X, acPuntoA.Value.Y + radioVector.GetPerpendicularVector().Y, 0));

                    // Agregamos los puntos a la Base de Datos de AutoCAD
                    acBlkTblRec.AppendEntity(puntoA);
                    acTrans.AddNewlyCreatedDBObject(puntoA, true);
                    acBlkTblRec.AppendEntity(puntoOrtogonal);
                    acTrans.AddNewlyCreatedDBObject(puntoOrtogonal, true);

                    // Mostrar el resultado
                    acEditor.WriteMessage($"\nEl ángulo de inclinación del radio vector es: {anguloEnGrados} grados.");
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
