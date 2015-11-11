require(["sitecore"], function(Sitecore) {
    if (Sitecore.ExperienceEditor === undefined || Sitecore.ExperienceEditor.PipelinesUtil === undefined) {
        define("myExperienceEditor", ["/-/speak/v1/ExperienceEditor/ExperienceEditor.js"], function(ExperienceEditor) {
            return ExperienceEditor;
        });
    } else {
        define("myExperienceEditor", [], function() {
            return Sitecore.ExperienceEditor;
        });
    }
});