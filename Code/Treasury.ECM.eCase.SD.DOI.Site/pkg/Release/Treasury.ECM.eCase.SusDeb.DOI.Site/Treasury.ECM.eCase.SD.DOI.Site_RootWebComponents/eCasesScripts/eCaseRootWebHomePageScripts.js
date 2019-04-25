        // Load Scripts for document.ready
        _spBodyOnLoadFunctionNames.push('eCaseReadyFunction');

        function eCaseReadyFunction() {
            if ($('#idHomePageNewItem').length) {
                // Get default "Add new item" link 
                var a_href = $('#idHomePageNewItem').attr('href');
                var lnkAddCase = $('#ctl00_PlaceHolderMain_lnkAddCase');
                if (lnkAddCase.length) {
                    // Assign link from "Add new item" to button 
                    lnkAddCase.attr('href', a_href);
                    lnkAddCase.click(function () {
                        ShowPopupDialog(a_href);
                        return false;
                    });
                }
                // Change default message for empty cases table
                var tdNoItems = $('#content-center-left .ms-vb:contains("There are no items to show in this view")');
                if (tdNoItems.length) {
                    var txtNoItemsMessage = 'To add a new case, click "Add a New Case"';
                    tdNoItems.text(txtNoItemsMessage);
                }
                // Fix display issue with Saved Searches Web Part
                if ($('.saved-searches-home').parents().eq(0).length) {
                    $('.saved-searches-home').parents().eq(0).css("margin-top", "-5px");
                }

            }
        };

        function openJudgeIssueSearch() {
            var options = SP.UI.$create_DialogOptions();
            options.url = "_layouts/eCaseSearch/JudgeIssueSearch.aspx";
            options.width = 600;
            options.height = 600;
            options.showMaximize = false;
            options.allowMaximize = false;
            SP.UI.ModalDialog.showModalDialog(options);
        }