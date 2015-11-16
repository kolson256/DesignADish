angular.module('designadish', [])

.controller('mainController', function($scope, $http) {

    $scope.formData = {};
    $scope.terms = {};

	$scope.selectedTerms = [];
	$scope.filterText = "";
		
	$scope.getRecipes = function($event) {
		
		//	Save search text for when filter criteria are selected
		if (!$scope.currentSearchText) {
			$scope.currentSearchText = $scope.formData;
		}
		
		//	$event is only passed in when filter criteria are selected
		if ($event) {
			
			//	Remove count from the filter text
			var filter = $event.currentTarget.text;
			filter = filter.substring(0, filter.indexOf("(") - 1);
			
			$scope.selectedTerms.push($event.currentTarget.id);
			
			//	Build the visual filter breadcrumb
			if ($scope.filterText == "") {
				$scope.filterText = filter;
			}
			else {
				$scope.filterText = $scope.filterText.concat(", ").concat(filter);
			}
		}
		
		var requestBody = {"searchText" : $scope.currentSearchText, "terms" : $scope.selectedTerms};
		
		//	Get list of recipes
		$http.post('http://75.98.175.117:51234/api/v1/recipes', requestBody)
			.success(function(data) {
				$scope.formData = {};
				$scope.allRecipes = data;
				//	Only show 20 recipes
				$scope.recipes = data.slice(0, 20);
			})
			.error(function(error) {
				console.log('Error: ' + error);
			});
			
		
		// Get top 20 ingredients
		$http.post('http://75.98.175.117:51234/api/v1/terms', requestBody)
			.success(function(data) {
				$scope.terms = data;
			})
			.error(function(error) {
				console.log('Error: ' + error);
			});
	}
	
	//	Reset the page
	$scope.clear = function() {
		$scope.currentSearchText = null;
		$scope.allRecipes = null;
		$scope.recipes = null;
		$scope.terms = null;
		$scope.selectedTerms = [];
		$scope.filterText = "";
	}
});