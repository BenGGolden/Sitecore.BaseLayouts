define(["sitecore"], function(Sitecore) {
    Sitecore.Commands.SelectBaseLayout =
    {
        canExecute: function(context) {
            if (!Sitecore.ExperienceEditor.isInMode("edit")) {
                return false;
            }

            return context.app.canExecute("BaseLayouts.CanExecute", context.currentContext);
        },
        execute: function(context) {
            context.app.disableButtonClickEvents();
            Sitecore.ExperienceEditor.PipelinesUtil.executePipeline(context.app.SelectBaseLayoutPipeline, function () {
                Sitecore.ExperienceEditor.PipelinesUtil.executeProcessors(Sitecore.Pipelines.SelectBaseLayout, context);
            });
            context.app.enableButtonClickEvents();
        }
    };
});