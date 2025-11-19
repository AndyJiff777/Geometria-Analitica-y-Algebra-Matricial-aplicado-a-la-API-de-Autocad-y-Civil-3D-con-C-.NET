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
    public class NombreComandosEjemplo05
    {
        // Creación del nombre del comando nuevo para AutoCAD
        [CommandMethod("ACL_VectPerpA")]
        // Creamos un Public Void para crear un Comando Nuevo en AutoCAD
        // Cada Public void, es un comando separado de AutoCAD
        public void ACL_VectPerpA()
        {
            // Obtenemos una Instancia del Documento y Solicita el acceso a un Documento activo de AutoCAD
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            // Obtenemos una Instancia de la Base de Datos y Solicita el acceso a la Base de Datos de AutoCAD
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

                // Opciones del primer punto A
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
                    // Opciones del segundo punto B
                    PromptPointOptions optsPuntoB = new PromptPointOptions("\n---> Clickar el segundo punto B: ");
                    optsPuntoB.UseBasePoint = true;
                    optsPuntoB.BasePoint = acPuntoA.Value;
                    optsPuntoB.UseDashedLine = true;
                    PromptPointResult acPuntoB = acEditor.GetPoint(optsPuntoB);

                    // Validar la respuesta del usuario
                    if(acPuntoB.Status!=PromptStatus.OK)
                    {
                        acEditor.WriteMessage("\n---> No se clickó el segundo punto B:");
                        return;
                    }
                    if (acPuntoB.Status==PromptStatus.OK)
                    {
                        // Opciones del segundo punto P
                        PromptPointOptions optsPuntoP = new PromptPointOptions("\n---> Clickar el segundo punto P: ");
                        // Capturamos el punto de la pantalla de AutoCAD
                        PromptPointResult acPuntoP = acEditor.GetPoint(optsPuntoP);

                        // Validar la respuesta del usuario
                        if (acPuntoP.Status != PromptStatus.OK)
                        {
                            acEditor.WriteMessage("\n---> No se clickó el segundo punto P:");
                            return;
                        }

                        if (acPuntoP.Status == PromptStatus.OK)
                        {
                            // Opciones del segundo punto P
                            PromptPointOptions optsPuntoQ = new PromptPointOptions("\n---> Clickar el segundo punto Q: ");
                            optsPuntoQ.UseBasePoint = true;
                            optsPuntoQ.BasePoint = acPuntoP.Value;
                            optsPuntoQ.UseDashedLine = true;
                            // Capturamos el punto de la pantalla de AutoCAD
                            PromptPointResult acPuntoQ = acEditor.GetPoint(optsPuntoQ);

                            // Validar la respuesta del usuario
                            if (acPuntoQ.Status != PromptStatus.OK)
                            {
                                acEditor.WriteMessage("\n---> No se clickó el segundo punto Q:");
                                return;
                            }

                            if (acPuntoQ.Status == PromptStatus.OK)
                            {
                                // (1) Crear el vector desde A hasta B, P y Q
                                Vector3d vectorAB = acPuntoB.Value - acPuntoA.Value;
                                Vector3d vectorPQ = acPuntoQ.Value - acPuntoP.Value;

                                // (2) Calcular el producto escalar entre los dos vectores
                                double productoEscalar = vectorAB.DotProduct(vectorPQ);

                                // (3) Verificar si los vectores son perpendiculares
                                if (Math.Abs(productoEscalar) < Tolerance.Global.EqualPoint)
                                {
                                    acEditor.WriteMessage("\n---> Los vectores AB y PQ son perpendiculares.");
                                }
                                else
                                {
                                    acEditor.WriteMessage("\n---> Los vectores AB y PQ NO son perpendiculares.");
                                }
                            }
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