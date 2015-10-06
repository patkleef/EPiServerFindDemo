<%@ Page Language="c#" Codebehind="ImportContent.aspx.cs" AutoEventWireup="False" Inherits="Site.Admin.ContentMigration" Title="" %>

<form runat="server">
    <asp:Button runat="server" ID="ImportButton" OnClick="ImportButton_Click" Text="Import content"/>
    
    <asp:Label runat="server" ID="ImportResult"></asp:Label>
    
    <asp:Button runat="server" ID="ReindexExternObject" OnClick="ReindexExternObjectButton_Click" Text="Reindex content"/>
</form>