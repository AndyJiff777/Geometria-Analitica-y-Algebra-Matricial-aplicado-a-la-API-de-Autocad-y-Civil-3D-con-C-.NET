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
    public class NombreComandoEjemplo04
    {
        // Creación del nombre del comando nuevo para AutoCAD
        [CommandMethod("ACL_VerParaleloB")]
        // Creamos un Public Void para crear un Comando Nuevo en AutoCAD
        // Cada Public void, es un comando separado de AutoCAD
        public void ACL_VerParaleloB()
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

                // Solicitar al usuario que seleccione la primera línea
                PromptEntityOptions optsLinea1 = new PromptEntityOptions("\n---> Seleccionar la primera línea: ");
                optsLinea1.SetRejectMessage("\n---> El objeto seleccionado no es una línea. Intente de nuevo.");
                optsLinea1.AddAllowedClass(typeof(Line), true);
                PromptEntityResult resLinea1 = acEditor.GetEntity(optsLinea1);

                if (resLinea1.Status != PromptStatus.OK)
                {
                    acEditor.WriteMessage("\n---> Selección cancelada.");
                    return;
                }
                if (resLinea1.Status == PromptStatus.OK)
                {
                    // Obtener la primera línea seleccionada
                    Line linea1 = acTrans.GetObject(resLinea1.ObjectId, OpenMode.ForRead) as Line;
                    // Solicitar al usuario que seleccione la segunda línea
                    PromptEntityOptions optsLinea2 = new PromptEntityOptions("\n---> Seleccionar la segunda línea: ");
                    optsLinea2.SetRejectMessage("\n---> El objeto seleccionado no es una línea. Intente de nuevo.");
                    optsLinea2.AddAllowedClass(typeof(Line), true);
                    PromptEntityResult resLinea2 = acEditor.GetEntity(optsLinea2);
                    if (resLinea2.Status != PromptStatus.OK)
                    {
                        acEditor.WriteMessage("\n---> Selección cancelada.");
                        return;
                    }
                    if (resLinea2.Status == PromptStatus.OK)
                    {
                        // Obtener la segunda línea seleccionada
                        Line linea2 = acTrans.GetObject(resLinea2.ObjectId, OpenMode.ForRead) as Line;
                        // Verificar si las líneas son paralelas
                        Vector3d dir1 = linea1.EndPoint - linea1.StartPoint;
                        Vector3d dir2 = linea2.EndPoint - linea2.StartPoint;
                        if (dir1.IsParallelTo(dir2))
                        {
                            acEditor.WriteMessage("\n---> Las líneas son paralelas.");
                        }
                        else
                        {
                            acEditor.WriteMessage("\n---> Las líneas no son paralelas.");
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