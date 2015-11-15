define(["sitecore", "/-/speak/v1/ExperienceEditor/ExperienceEditor.js"], function(Sitecore, ExperienceEditor) {
    Sitecore.Commands.SelectBaseLayout =
    {
        canExecute: function(context) {
            if (!ExperienceEditor.isInMode("edit")) {
                return false;
            }
            return context.app.canExecute("BaseLayouts.CanExecute", context.currentContext);
        },
        execute: function(context) {
            context.app.disableButtonClickEvents();
            ExperienceEditor.PipelinesUtil.executePipeline(context.app.SelectBaseLayoutPipeline, function() {
                ExperienceEditor.PipelinesUtil.executeProcessors(Sitecore.Pipelines.SelectBaseLayout, context);
            });
            context.app.enableButtonClickEvents();
        }
    };
});