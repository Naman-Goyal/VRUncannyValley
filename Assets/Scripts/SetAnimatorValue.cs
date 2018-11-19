using UnityEngine;
using System.Collections;

public class SetAnimatorValue : MonoBehaviour {
	//Caso quiser testar no Start()
	//Start()でテストしたい時用
	//If you want to test at Start ()
	public bool testAtStart;
	//o Nome e o Valor do [Parameter] que você quer modificar dividindo eles com o virgula ! exp : [Nome,Valor]
	//セットしたいパラメータ名と[,]で区切ってセットしたい値 例 : [パラメータ名,値]
	//The Name and the Value of the [Parameter] you want to modify dividing them with comma ! exp : [Name,Value]
	public string nameValue;
	//Para acessar no Animator
	//Animatorへのアクセス
	//To access the Animator
	Animator anim;


	// Use this for initialization
	void Start () {
		if(testAtStart)
			SetParameterValue(nameValue);
	}
	//Para definir o valor atraves de outros Scripts
	//外部からの呼び出しで値をセット
	//To set the value through other scripts
	public void SetParameterValue(string nameAndValue){
		//Dividir o [nameAndValue] em dois atraves a [,] virgula 
		//セットしたいパラメータ名とセットしたい値を[,]で区切って小分け
		//Divide the [nameAndValue] in two through the [,] comma
		//Example : nameValue = Move,1.0 --->  list[0] = Move ,  list[1] = 1.0
		string[] list = nameAndValue.Split(',');
		//Debug de cada valor na lista
		//小分けにしたのを全てDebugで表示
		//Debug of each value in the list
		/*foreach (string item in list) {
			Debug.Log(item);
		}*/
		//Acessar no Animator
		//Animatorへのアクセス
		//Access the Animator
		anim = GetComponent<Animator> ();
		//Acessar na lista de Parameter do Animator
		//Animatorの各パラメータにアクセス
		//Access to the Parameter list of Animator
		AnimatorControllerParameter[] myParam = anim.parameters;
		//Vou usar para procurar o [Parameter] que quero modificar
		//これを使ってnameValueで指定したパラメータが存在するかを判断する
		//I will use to search for the [parameter] I want to change
		int paramIndex = -1;
		//Vou usar o for() para saber em que lugar da list esta o [Parameter] para usar no anim.GetParameter
		//anim.GetParameterで何番のparameterにアクセスしたいのか指定する為にfor()を行う
		//I will use for () to know where in the list this the [Parameter] to use the anim.GetParameter
		for (int i = 0; i < myParam.Length; i++) {
			if(myParam[i].name == list[0]){
				paramIndex = i;
			}
		}
		//Se nao achar o [Parameter] que quero isso quer diser que nao existe
		//paramIndexの値に変化なければその値は存在しない
		//If I do not find the [parameter] I want that means it does not exist
		if(paramIndex == -1){
			Debug.Log("Can't find that Parameter....");
			//Se ecncontrar-lo
			//その値が存在すれば
			//If find that
		}else{
			//Debug.Log(myParam[paramIndex].name + " : " + paramIndex);

			//Vou definir o Valor do [Parameter] dependendo do [type] do valor
			//その値のtypeによってパラメータの値のセット方法を変える
			//I will set the value of [parameter] depending of the [type] of that value
			switch (anim.GetParameter(paramIndex).type) {

			case AnimatorControllerParameterType.Bool :
				//Debug.Log("This is Bool");
				anim.SetBool(list[0], (list[1] == "true"));
				break;

			case AnimatorControllerParameterType.Float :
				//Debug.Log("This is Float");
				anim.SetFloat(list[0], float.Parse(list[1]));
				break;

			case AnimatorControllerParameterType.Int :
				//Debug.Log("This is Int");
				anim.SetInteger(list[0], int.Parse(list[1]));
				break;

			case AnimatorControllerParameterType.Trigger :
				//Debug.Log("This is Trigger");
				anim.SetTrigger(list[0]);
				break;
			}
		}
	}

}
