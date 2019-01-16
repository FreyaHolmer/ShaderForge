using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ShaderForge{


	public class DependencyTree<T> where T : IDependable<T>{

		public List<IDependable<T>> tree;

		public DependencyTree(){
			tree = new List<IDependable<T>>();
		}



		public void Add(params IDependable<T>[] deps){

		}

		public void Add(IDependable<T> dep){
			AddUnique(dep);
			foreach(IDependable<T> d in dep.Dependencies){
				AddUnique(d);
			}
		}

		private void AddUnique(IDependable<T> dep){
			if(!tree.Contains(dep)){
				tree.Add(dep);
			}
		}

		/*
		public void Add(T obj){
			tree.Add(obj);
		}

		public void Add(List<IDependable<T>> objs){
			tree.AddRange(objs);
		}
	*/
		public void Sort(){
			AssignDepthValues();
			SortByDepth();
		}
		
		private void MoveUpNode(IDependable<T> dp, bool initial){
			if(!initial)
				dp.Depth++;
			foreach(IDependable<T> d in dp.Dependencies){
				if(d.Depth <= dp.Depth){
					MoveUpNode(d, initial:false);
				}
			}
		}

		private void AssignDepthValues(){
			ResetNodeDepths();
			foreach(IDependable<T> dp in tree)
				MoveUpNode(dp, initial:true);
		}

		private void SortByDepth(){
			tree.OrderBy(o=>o.Depth).ToList();
		}

		private void ResetNodeDepths(){
			foreach(IDependable<T> dp in tree)
				dp.Depth = 0;
		}


		public List<List<T>> GetDependenciesByGroup(out int maxWidth){
			List<List<T>> groups = new List<List<T>>();
			maxWidth = 0;

			int groupCount = tree.GroupBy(p => p.Depth).Select(g => g.First()).Count();
			
			for(int i=0;i<groupCount;i++){
				groups.Add(tree.Select(x=>(T)x).Where(x=>x.Depth == i).ToList());
				maxWidth = Mathf.Max(maxWidth, groups[i].Count);
			}
			
			return groups;
		}




	}


	public interface IDependable<T>{
		int Depth { get; set; }
		List<T> Dependencies { get; set;}
		void AddDependency(T dp);
	}

}