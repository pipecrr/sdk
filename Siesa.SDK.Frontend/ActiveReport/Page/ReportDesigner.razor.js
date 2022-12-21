window.initDesigner = function () {
    var designerOptions = GrapeCity.ActiveReports.WebDesigner.createDesignerOptions();
    designerOptions.server.url = 'api';
    designerOptions.reportInfo = null;

    designerOptions.openButton.visible = true;
    designerOptions.saveButton.visible = true;
    designerOptions.saveAsButton.visible = true;

    // Used in About dialog and File View Help tab.
    designerOptions.documentation.home = 'https://www.grapecity.com/activereports/docs/v15/online-webdesigner/overview.html';
    // Used in notifications about report items transformation.
    designerOptions.documentation.reportItemsTransformation = 'https://www.grapecity.com/activereports/docs/v15/online-webdesigner/supportedcontrols.html';

    designerOptions.reportItemsFeatures.table.autoFillFooter = true;

    var viewer = null;
    designerOptions.openViewer = function (options) { 
        if (viewer) {
            viewer.openReport(options.reportInfo.id);
            return;
        }
        viewer = GrapeCity.ActiveReports.JSViewer.create({
            locale: options.locale,
            element: '#' + options.element,
            reportService: {
                url: 'api/reporting',
            },
            reportID: options.reportInfo.id,
            settings: {
                zoomType: 'FitPage'
            },
        });
    };

    designerOptions.dataTab.dataSets.canModify = true;
    designerOptions.dataTab.dataSources.canModify = true;

    GrapeCity.ActiveReports.WebDesigner.renderApplication('designer-id', designerOptions);
}

