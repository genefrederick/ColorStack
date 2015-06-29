<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="SendColors.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="StyleSheet1.css" rel="stylesheet" type="text/css" />
</head>
<body>
        <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
            <div class="left">
                <asp:RadioButtonList ID="RadioButtonList1" runat="server">
                    <asp:ListItem Value="Red">Red</asp:ListItem>
                    <asp:ListItem Value="Orange">Orange</asp:ListItem>
                    <asp:ListItem Value="Yellow">Yellow</asp:ListItem>
                    <asp:ListItem Value="Green">Green</asp:ListItem>
                    <asp:ListItem Value="Blue">Blue</asp:ListItem>
                    <asp:ListItem Value="Indigo">Indigo</asp:ListItem>
                    <asp:ListItem Value="Violet">Violet</asp:ListItem>
                </asp:RadioButtonList>
                <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Submit" />
            </div>

            <div class="left">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Timer ID="Timer1" runat="server" Interval="1000" OnTick="Timer1_Tick1">

                        </asp:Timer>
                        <div id="sentcolorsholder" runat="server">
        
                        </div>

                        <div id="receivedcolorsholder" class="right" runat="server">
        
                        </div>      
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

        </form>
</body>
</html>
