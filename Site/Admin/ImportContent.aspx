<%@ Page Language="c#" Codebehind="ImportContent.aspx.cs" AutoEventWireup="False" Inherits="Site.Admin.ContentMigration" Title="" %>

<form runat="server">
    <table>
        <tr>
            <td>Rss 1</td>
            <td><asp:TextBox ID="Rss1TextBox" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>Rss 2</td>
            <td><asp:TextBox ID="Rss2TextBox" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>Rss 3</td>
            <td><asp:TextBox ID="Rss3TextBox" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>Rss 4</td>
            <td><asp:TextBox ID="Rss4TextBox" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>Rss 5</td>
            <td><asp:TextBox ID="Rss5TextBox" runat="server"></asp:TextBox></td>
        </tr>
    </table>
    <asp:Button runat="server" ID="ImportButton" OnClick="ImportButton_Click" Text="Import content"/>
    
    <asp:Label runat="server" ID="ImportResult"></asp:Label>
    
    <asp:Button runat="server" ID="ReindexExternObject" OnClick="ReindexExternObjectButton_Click" Text="Reindex content"/>
</form>