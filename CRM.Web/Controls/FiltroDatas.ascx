<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FiltroDatas.ascx.cs" Inherits="CRM.Web.Controls.FiltroDatas" %>

<div class="row g-2 align-items-end">
    <div class="col-auto">
        <label class="form-label small text-muted mb-1">De</label>
        <asp:TextBox ID="txtDataInicial" runat="server" CssClass="form-control form-control-sm" TextMode="Date" />
    </div>
    <div class="col-auto">
        <label class="form-label small text-muted mb-1">Até</label>
        <asp:TextBox ID="txtDataFinal" runat="server" CssClass="form-control form-control-sm" TextMode="Date" />
    </div>
    <div class="col-auto">
        <asp:CompareValidator ID="cvDatas" runat="server"
            ControlToValidate="txtDataFinal" ControlToCompare="txtDataInicial"
            Operator="GreaterThanEqual" Type="Date"
            Display="Dynamic" CssClass="text-danger small d-block mt-1"
            ErrorMessage="A data final não pode ser anterior à inicial." />
    </div>
</div>