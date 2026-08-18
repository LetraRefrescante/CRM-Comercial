-- =============================================
-- Script: 015_Settings_MonthlySalesTarget.sql
-- Descrição: Acrescenta o objetivo comercial mensal, usado no Dashboard para o
--            indicador "Objetivo comercial e percentagem alcançada".
--            Decisão: valor único à escala da empresa (não por comercial) - é o
--            que dá para ter pronto rapidamente; evolui para tabela própria com
--            histórico por utilizador se um dia for preciso.
-- =============================================

USE CRM;
GO

ALTER TABLE Settings ADD MonthlySalesTarget DECIMAL(18,2) NULL;
GO