using System;
using System.Collections.Generic;
using DB;
using System.Data.SQLite;
namespace OrderBot
{
    public class Order
    {
        private enum State
        {
            WELCOMING, DETAILS, listDepartment, storeDepartmentId
        }

        private State nCur = State.WELCOMING;
        private string userInput;
        private int departmentId=0;
        private List<string> productList = new List<string>();
        private HomeDepotDB dbObject;

        public Order()
        {
            dbObject = new HomeDepotDB();
            string productListQuery = "select * from product";
            SQLiteCommand queryCommnad = new SQLiteCommand(productListQuery, dbObject.conn);
            dbObject.StartConnection();
            SQLiteDataReader result = queryCommnad.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    productList.Add(result["name"].ToString());
                }
            }
        }

        private State currentStateIdentifier(String sInMessage, State nCur)
        {

            if (sInMessage.Contains("")) { 
            }
            return nCur;
        }

        public String OnMessage(String sInMessage)
        {
            String sMessage = "Welcome! Looking to find a product at Home Depot?\n";
            switch (this.nCur)
            {
                case State.WELCOMING:
                    this.nCur = State.listDepartment;
                    break;

                case State.listDepartment:
                    if ("No bye goodbye nope byee byeee Noo tata".ToLower().Contains(sInMessage.ToLower()))
                    {
                        sMessage = "Have a good day!";
                        this.nCur = State.WELCOMING;
                        break;
                    } 
                    string departmentQuery = "select * from department;";
                    SQLiteCommand deptCommnad = new SQLiteCommand(departmentQuery, dbObject.conn);
                    dbObject.StartConnection();
                    SQLiteDataReader departments = deptCommnad.ExecuteReader();
                    int i = 0;
                    if (departments.HasRows)
                    {
                        while (departments.Read())
                        {
                            if (i == 0)
                            {
                                sMessage = "Please enter a department number from the departments listed below: \n";
                                i++;
                                sMessage+= i+". "+departments["name"];
                            }
                            else
                            {
                                i++;
                                sMessage += "\n"+i + ". " + departments["name"];
                            }
                            System.Diagnostics.Debug.WriteLine(sMessage);
                        }
                    }
                    i = 0;
                    this.nCur = State.storeDepartmentId;
                    break;
                case State.storeDepartmentId:
                   
                    try
                    {
                        if ("No bye goodbye nope byee byeee Noo tata".ToLower().Contains(sInMessage.ToLower()))
                        {
                            sMessage = "Have a good day!";
                            this.nCur = State.WELCOMING;
                            break;
                        }
                        int deptId = Int32.Parse(sInMessage);
                        System.Diagnostics.Debug.WriteLine("Ok" + sInMessage);
                        string departmentSearchQuery = "select * from department where id=" + deptId;
                        SQLiteCommand deptSearchCommnad = new SQLiteCommand(departmentSearchQuery, dbObject.conn);
                        dbObject.StartConnection();
                        SQLiteDataReader departmentsExist = deptSearchCommnad.ExecuteReader();
                        if (!departmentsExist.HasRows)
                        {
                            this.departmentId = 0;
                            sMessage = "Invalid Department Selected, would you like to retry?";
                            this.nCur = State.listDepartment;
                        }
                        while (departmentsExist.Read())
                        {
                            System.Diagnostics.Debug.WriteLine(departmentsExist[1]);
                            this.departmentId = deptId;
                            sMessage = "Please provide the name of the product you are trying to locate in our store";
                            this.nCur = State.DETAILS;
                        }
                    }
                    catch(Exception e)
                    {
                        sMessage = "Please enter a correct department number (in integer).";
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                    break;
                case State.DETAILS:
                    try
                    {
                        if("No bye goodbye nope byee byeee Noo tata".ToLower().Contains(sInMessage.ToLower()))
                        {
                            sMessage = "Have a good day!";
                            this.nCur = State.WELCOMING;
                            break;
                        }
                        List<string> whereProducts = new List<string>();
                        userInput = sInMessage.ToLower();
                        foreach (var item in productList)
                        {
                            if (userInput.Contains(item.ToLower()))
                            {
                                whereProducts.Add(item);
                            }
                        }
                        string wherequery = "";
                        for(var j=0;j<whereProducts.Count;j++)
                        {
                            if (j==0)
                            {
                                wherequery += " where name='"+ whereProducts[j] +"'";
                            } else
                            {
                                wherequery += "OR name='" + whereProducts[j] + "'";
                            }
                        }
                        if (wherequery.Length > 1)
                        {
                            if (departmentId == 0)
                            {
                                this.nCur = State.listDepartment;
                            }
                            wherequery += " and dept_id=" + departmentId+";";
                            string searchQuery = "select * from product"+wherequery;
                            System.Diagnostics.Debug.WriteLine(searchQuery);
                            SQLiteCommand searchCommnad = new SQLiteCommand(searchQuery, dbObject.conn);
                            dbObject.StartConnection();
                            SQLiteDataReader result = searchCommnad.ExecuteReader();
                            int k = 0;
                            if (result.HasRows)
                            {
                                while (result.Read())
                                {
                                    if (k == 0)
                                    {
                                        k++;
                                        sMessage = "Product: " + result["name"] + " can be found in " + result["location"];
                                    } else
                                    {
                                        sMessage += "\nProduct: " + result["name"] + " can be found in " + result["location"];
                                    }
                                    System.Diagnostics.Debug.WriteLine(sMessage);
                                }
                            }
                            k = 0;
                        } else if("Thank you thanks okay got it fine nice".ToLower().Contains(sInMessage.ToLower()))
                        {
                            sMessage = "Your welcome!";
                            this.nCur = State.WELCOMING;
                            break;
                        }
                        else
                        {
                            sMessage = "No such product exists in our inventory, Please retry!";
                            this.nCur = State.listDepartment;
                            break;
                        }
                    } catch (Exception)
                    {
                        sMessage = "Sorry I was not able to find the product you are looking for, please try again!";
                        this.nCur = State.listDepartment;
                        break;
                    }
                    this.nCur = State.DETAILS;
                    break;
            }
            System.Diagnostics.Debug.WriteLine(sMessage);
            return sMessage;
        }

    }
}