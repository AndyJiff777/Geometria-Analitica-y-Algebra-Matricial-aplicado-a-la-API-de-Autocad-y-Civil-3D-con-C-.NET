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
    public class NombreComandosEjemploAP01Metodo1
    {
        // Creación del nombre del comando nuevo para AutoCAD
        [CommandMethod("ACL_AP01Met01")]
        // Creamos un Public Void para crear un Comando Nuevo en AutoCAD
        // Cada Public void, es un comando separado de AutoCAD
        public void ACL_AP01Met01()
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
                        // Ingresamos la distandia desde el punto P a la línea AB                        
                        PromptDoubleOptions optsDistancia = new PromptDoubleOptions("\n---> Ingresar la distancia desde el punto P a la línea AB: ");
                        optsDistancia.AllowNegative = true;
                        optsDistancia.AllowZero = false;
                        optsDistancia.DefaultValue = 10.0;
                        PromptDoubleResult acDistancia = acEditor.GetDouble(optsDistancia);

                        if (acDistancia.Status == PromptStatus.OK)
                        {
                            // (1) Hallar el vector AB
                            Vector3d vectorAB = acPuntoB.Value - acPuntoA.Value;
                            double Xab = vectorAB.X;
                            double Yab = vectorAB.Y;

                            // (2) Hallar el vector perpendicular a AB en el plano XY
                            double perp_dx = -Yab;
                            double perp_dy = Xab;

                            // (3) Hallamos el vector unitario perpendicular AB
                            double moduloAB = Math.Sqrt(perp_dx * perp_dx + perp_dy * perp_dy);
                            double ux = perp_dx / moduloAB;
                            double uy = perp_dy / moduloAB;

                            // (4) Multiplicamos el vector unitario por la distancia deseada (distancia desde P a la línea AB)
                            double uxD = ux * acDistancia.Value;
                            double uyD = uy * acDistancia.Value;

                            // (5) Sumar el vector unitario hallado al punto P
                            // Coordenadas del punto proyectado P
                            double Xp = acPuntoA.Value.X + uxD;
                            double Yp = acPuntoA.Value.Y + uyD;
                            Point3d P = new Point3d(Xp, Yp, 0);
                            // Coordenadas del punto proyectado Q
                            double Xq = acPuntoB.Value.X + uxD;
                            double Yq = acPuntoB.Value.Y + uyD;
                            Point3d Q = new Point3d(Xq, Yq, 0);

                            // Crear la línea entre AB
                            Line lineaAB = new Line(acPuntoA.Value, acPuntoB.Value);

                            // Crear el punto proyectado P y Q, agregarlo al espacio modelo
                            // Línea AB
                            acBlkTblRec.AppendEntity(lineaAB);
                            acTrans.AddNewlyCreatedDBObject(lineaAB, true);
                            // Punto proyectado P
                            DBPoint puntoProyP = new DBPoint(P);
                            acBlkTblRec.AppendEntity(puntoProyP);
                            acTrans.AddNewlyCreatedDBObject(puntoProyP, true);
                            // Punto proyectado Q
                            DBPoint puntoProyQ = new DBPoint(Q);
                            acBlkTblRec.AppendEntity(puntoProyQ);
                            acTrans.AddNewlyCreatedDBObject(puntoProyQ, true);
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
