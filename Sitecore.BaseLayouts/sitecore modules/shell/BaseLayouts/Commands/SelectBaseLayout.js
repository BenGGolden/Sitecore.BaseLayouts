require(["/sitecore modules/shell/BaseLayouts/ExperienceEditorShim.js"], function() {
    require(["sitecore", "myExperienceEditor"], function(sitecore, experienceEditor) {
        sitecore.Commands.SelectBaseLayout =
        {
            canExecute: function(context) {
                if (!experienceEditor.isInMode("edit")) {
                    return false;
                }
                return context.app.canExecute("BaseLayouts.CanExecute", context.currentContext);
            },
            execute: function(context) {
                context.app.disableButtonClickEvents();
                experienceEditor.PipelinesUtil.executePipeline(context.app.SelectBaseLayoutPipeline, function() {
                    experienceEditor.PipelinesUtil.executeProcessors(sitecore.Pipelines.SelectBaseLayout, context);
                });
                context.app.enableButtonClickEvents();
            }
        };
    });
});